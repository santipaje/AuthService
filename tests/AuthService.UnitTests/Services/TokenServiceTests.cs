
using AuthService.Application.DTOs;
using AuthService.Infrastructure.Identity;
using AuthService.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AuthService.UnitTests.Services
{
    /// <summary>
    /// Testing class for the Token Service
    /// </summary>
    public class TokenServiceTests
    {

        #region Context

        /// <summary>
        /// Set up the context
        /// </summary>
        private static class TestContext
        {
            public static JwtSecurityTokenHandler Handler { get; } = new();

            public static readonly JwtSettings DefaultSettings = new()
            {
                Issuer = "AuthTest",
                Audience = "AuthTest",
                Key = "THIS_IS_A_TEST_KEY_FOR_UNIT_TESTS_123!",
                DurationInMinutes = 60,
            };

            /// <summary>
            /// Creates System Under Test
            /// </summary>
            /// <param name="settings"></param>
            /// <returns></returns>
            public static TokenService CreateSut(JwtSettings? settings = null)
            {
                return new TokenService(Options.Create(settings ?? DefaultSettings));
            }

            /// <summary>
            /// Create User DTO
            /// </summary>
            /// <param name="roles"></param>
            /// <returns></returns>
            public static UserInfoDto CreateUser(IList<string>? roles = null)
            {
                return new UserInfoDto
                {
                    Id = "1",
                    FullName = "test-full-name",
                    UserName = "test-user-name",
                    Email = "user@test.com",
                    Roles = roles ?? []
                };
            }

            /// <summary>
            /// Auxiliar method to read token
            /// </summary>
            /// <param name="token"></param>
            /// <returns></returns>
            public static JwtSecurityToken ReadToken(string token)
            {
                return Handler.ReadJwtToken(token);
            }
        }

        #endregion

        [Fact]
        public void GenerateToken_Should_Return_Valid_Jwt_With_Expected_Claims()
        {
            // Arrange
            var sut = TestContext.CreateSut();
            var user = TestContext.CreateUser(["Admin", "User"]);

            // Act
            var token = sut.GenerateToken(user);
            var jwt = TestContext.ReadToken(token);

            // Assert
            // Verify claims
            jwt.Claims.Should().NotBeEmpty();
            jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Name)?.Value.Should().Be("test-full-name");
            jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.PreferredUsername)?.Value.Should().Be("test-user-name");
            jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)?.Value.Should().Be("user@test.com");

            // Verify roles
            var roleClaims = jwt.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
            roleClaims.Should().ContainInOrder("Admin", "User");
        }

        [Fact]
        public void GenerateToken_Should_Set_Expiration_Correctly()
        {
            // Arrange
            var sut = TestContext.CreateSut();
            var user = TestContext.CreateUser();

            // Act
            var token = sut.GenerateToken(user);
            var jwt = TestContext.ReadToken(token);

            // Assert
            // Verify expiration time
            jwt.ValidTo.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(60), TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void GenerateToken_Should_Work_With_No_Roles()
        {
            // Arrange
            var sut = TestContext.CreateSut();
            var user = TestContext.CreateUser();

            // Act
            var token = sut.GenerateToken(user);
            var jwt = TestContext.ReadToken(token);

            // Assert
            jwt.Claims.Any(c => c.Type == ClaimTypes.Role).Should().BeFalse();
        }

        [Fact]
        public void GenerateToken_Should_Throw_ArgumentException_When_SecurityKey_Is_Too_Short()
        {
            // Arrange
            var shortKeySettings = new JwtSettings
            {
                Issuer = "AuthTest",
                Audience = "AuthTest",
                Key = "SHORT_KEY", 
                DurationInMinutes = 60
            };
            var sut = TestContext.CreateSut(shortKeySettings);
            var user = TestContext.CreateUser();

            // Act & Assert
            // Token generation will throw an exception as the key does not meet the minimum length requirement
            sut.Invoking(s => s.GenerateToken(user))
               .Should().Throw<Exception>();
        }

    }
}
