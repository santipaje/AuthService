using AuthService.Application.Common.Constants;
using AuthService.Application.DTOs;
using AuthService.Application.DTOs.Requests;
using AuthService.Application.Interfaces;
using AuthService.Application.Validators;
using AuthService.Infrastructure.Identity;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;

namespace AuthService.UnitTests.Services
{
    /// <summary>
    /// Testing class for the Auth Service
    /// </summary>
    public class AuthServiceTests
    {

        public AuthServiceTests() { }

        #region Context

        public class TestContext
        {
            public Mock<UserManager<ApplicationUser>> UserManagerMock { get; }
            public Mock<ITokenService> TokenServiceMock { get; }
            public Mock<IValidator<RegisterRequestDto>> RegisterValidatorMock { get; }
            public Mock<IValidator<LoginRequestDto>> LoginValidatorMock { get; }

            /// <summary>
            /// AuthService System Under Test
            /// </summary>
            public Infrastructure.Services.AuthService Sut { get; }

            /// <summary>
            /// Set up the context
            /// </summary>
            public TestContext()
            {
                UserManagerMock = CreateUserManagerMock();
                TokenServiceMock = new Mock<ITokenService>();
                RegisterValidatorMock = new Mock<IValidator<RegisterRequestDto>>();
                LoginValidatorMock = new Mock<IValidator<LoginRequestDto>>();

                // Default configuration: validators always pass (Happy Path defautl)
                RegisterValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<RegisterRequestDto>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new ValidationResult());
                LoginValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<LoginRequestDto>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new ValidationResult());

                var jwtSettings = Options.Create(new JwtSettings
                {
                    Issuer = "AuthTest",
                    Audience = "AuthTest",
                    Key = "THIS_IS_A_TEST_KEY_FOR_UNIT_TESTS_123!",
                    DurationInMinutes = 60,
                });

                Sut = new Infrastructure.Services.AuthService(
                    UserManagerMock.Object,
                    TokenServiceMock.Object,
                    RegisterValidatorMock.Object,
                    LoginValidatorMock.Object,
                    jwtSettings
                );
            }

            private static Mock<UserManager<ApplicationUser>> CreateUserManagerMock()
            {
                var store = new Mock<IUserStore<ApplicationUser>>();
                return new Mock<UserManager<ApplicationUser>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
            }
        }

        #endregion

        #region Register

        [Fact]
        [Trait("Category", "Register")]
        public async Task Register_Should_Return_Error_When_Email_Exists()
        {
            // Arrange
            var ctx = new TestContext();

            ctx.UserManagerMock.Setup(x => x.FindByEmailAsync("exists@test.com"))
                .ReturnsAsync(new ApplicationUser { Email = "exists@test.com" });

            var request = new RegisterRequestDto { Email = "exists@test.com", Password = "Password1234!", FullName = "test-full-name", UserName = "test-user-name" };

            // Act
            var result = await ctx.Sut.RegisterAsync(request);

            // Assert
            result.Succeeded.Should().BeFalse();
            result.Errors.Should().Contain("Email already registered.");

            // Verify It doesn't tried to create the user
            ctx.UserManagerMock.Verify(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
        }


        [Fact]
        [Trait("Category", "Register")]
        public async Task Register_Should_Create_User_When_Valid()
        {
            // Arrange
            var ctx = new TestContext();

            ctx.UserManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser?)null);

            ctx.UserManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            ctx.UserManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var request = new RegisterRequestDto { Email = "good@test.com", Password = "Password123!", FullName = "test-full-name", UserName = "user" };

            // Act
            var result = await ctx.Sut.RegisterAsync(request);

            // Assert
            result.Succeeded.Should().BeTrue();

            // Verify critic methods in the creation
            ctx.UserManagerMock.Verify(x => x.CreateAsync(It.Is<ApplicationUser>(u => u.Email == request.Email), request.Password), Times.Once);
            ctx.UserManagerMock.Verify(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), RoleNames.User), Times.Once);
        }

        #endregion

        #region Login

        [Fact]
        [Trait("Category", "Login")]
        public async Task Login_Should_Fail_When_Password_Is_Wrong()
        {
            // Arrange
            var ctx = new TestContext();

            ctx.UserManagerMock.Setup(x => x.FindByEmailAsync("wrong@test.com"))
                .ReturnsAsync(new ApplicationUser { Email = "wrong@test.com" });

            ctx.UserManagerMock.Setup(x => x.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            var loginRequest = new LoginRequestDto { Email = "wrong@test.com", Password = "bad" };

            // Act
            var result = await ctx.Sut.LoginAsync(loginRequest);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        [Trait("Category", "Login")]
        public async Task Login_Should_Return_Token_When_Credentials_Are_Valid()
        {
            // Assert
            var ctx = new TestContext();
            var user = new ApplicationUser { Email = "ok@test.com" };

            ctx.UserManagerMock.Setup(x => x.FindByEmailAsync("ok@test.com"))
                .ReturnsAsync(user);

            ctx.UserManagerMock.Setup(x => x.CheckPasswordAsync(user, "Password1234!"))
                .ReturnsAsync(true);

            ctx.UserManagerMock.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(["Admin"]);

            ctx.TokenServiceMock.Setup(x => x.GenerateToken(It.IsAny<UserInfoDto>()))
                .Returns("FAKE_TOKEN");

            var loginRequest = new LoginRequestDto { Email = "ok@test.com", Password = "Password1234!" };

            // Act
            var result = await ctx.Sut.LoginAsync(loginRequest);

            // Assert
            result.Should().NotBeNull();
            result.AccessToken.Should().Be("FAKE_TOKEN");

            // Verify TokenService have been called
            ctx.TokenServiceMock.Verify(x => x.GenerateToken(It.Is<UserInfoDto>(u => u.Email == user.Email)), Times.Once);
        }

        #endregion

    }
}
