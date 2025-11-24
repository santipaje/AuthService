using AuthService.Application.DTOs;
using AuthService.Application.DTOs.Requests;
using AuthService.Application.Interfaces;
using AuthService.Application.Validators;
using AuthService.Infrastructure.Identity;
using FluentAssertions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;

namespace AuthService.UnitTests.Services
{
    public class AuthServiceTests
    {
        private Mock<UserManager<ApplicationUser>> _userManagerMock;
        private Mock<ITokenService> _tokenServiceMock;
        private Mock<RegisterRequestValidator> _registerRequestValidatorMock;
        private Mock<LoginRequestValidator> _loginRequestValidatorMock;

        public AuthServiceTests()
        {
            _userManagerMock = CreateUserManagerMock();
            _tokenServiceMock = new Mock<ITokenService>();
            _registerRequestValidatorMock = new();
            _loginRequestValidatorMock = new();
        }

        #region Mocks

        private Mock<UserManager<ApplicationUser>> CreateUserManagerMock()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();

            return new Mock<UserManager<ApplicationUser>>(
                store.Object,
                null, null, null, null, null, null, null, null);
        }

        private Infrastructure.Services.AuthService CreateAuthService()
        {
            var jwtSettings = Options.Create(new JwtSettings
            {
                Issuer = "AuthService",
                Audience = "AuthServiceClient",
                Key = "THIS_IS_A_TEST_KEY_FOR_UNIT_TESTS_123!",
                DurationInMinutes = 60,
            });

            return new Infrastructure.Services.AuthService(
                _userManagerMock.Object,
                _tokenServiceMock.Object,
                _registerRequestValidatorMock.Object,
                _loginRequestValidatorMock.Object,
                jwtSettings
            );
        }

        #endregion

        // Register

        [Fact]
        public async Task Register_Should_Return_Error_When_Email_Exists()
        {
            // Arrange
            _registerRequestValidatorMock.Setup(x => x.ValidateAsync(It.IsAny<RegisterRequestDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _userManagerMock.Setup(x => x.FindByEmailAsync("exists@test.com"))
                .ReturnsAsync(new ApplicationUser { Email = "exists@test.com" });

            var authService = CreateAuthService();

            var registerRequest = new RegisterRequestDto
            {
                Email = "exists@test.com",
                Password = "Password1234!",
                UserName = "test-user-name"
            };

            // Act
            var result = await authService.RegisterAsync(registerRequest);

            // Assert
            result.Succeeded.Should().BeFalse();
            result.Errors.Should().Contain("Email already registered.");
        }


        [Fact]
        public async Task Register_Should_Create_User_When_Valid()
        {
            // Arrange
            _registerRequestValidatorMock.Setup(x => x.ValidateAsync(It.IsAny<RegisterRequestDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser?)null);

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var authService = CreateAuthService();

            var registerRequest = new RegisterRequestDto
            {
                Email = "good@test.com",
                Password = "Password123!",
                UserName = "user"
            };

            // Act
            var result = await authService.RegisterAsync(registerRequest);

            // Assert
            result.Succeeded.Should().BeTrue();
        }

        // Login

        [Fact]
        public async Task Login_Should_Fail_When_Password_Is_Wrong()
        {
            // Arrange
            _loginRequestValidatorMock.Setup(x => x.ValidateAsync(It.IsAny<LoginRequestDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _userManagerMock.Setup(x => x.FindByEmailAsync("wrong@test.com"))
                .ReturnsAsync(new ApplicationUser { Email = "wrong@test.com" });

            _userManagerMock.Setup(x => x.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            var authService = CreateAuthService();

            var loginRequest = new LoginRequestDto { Email = "wrong@test.com", Password = "bad" };

            // Act
            var result = await authService.LoginAsync(loginRequest);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task Login_Should_Return_Token_When_Credentials_Are_Valid()
        {
            // Assert
            var user = new ApplicationUser { Email = "ok@test.com" };

            _loginRequestValidatorMock.Setup(x => x.ValidateAsync(It.IsAny<LoginRequestDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _userManagerMock.Setup(x => x.FindByEmailAsync("ok@test.com"))
                .ReturnsAsync(user);

            _userManagerMock.Setup(x => x.CheckPasswordAsync(user, "Password1234!"))
                .ReturnsAsync(true);

            _userManagerMock.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(["Admin"]);

            _tokenServiceMock.Setup(x => x.GenerateToken(It.IsAny<UserInfoDto>()))
                .Returns("FAKE_TOKEN");

            var authService = CreateAuthService();

            var loginRequest = new LoginRequestDto { Email = "ok@test.com", Password = "Password1234!" };

            // Act
            var result = await authService.LoginAsync(loginRequest);

            // Assert
            result.Should().NotBeNull();
            result.AccessToken.Should().Be("FAKE_TOKEN");
        }

    }
}
