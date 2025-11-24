
using AuthService.Application.DTOs;
using AuthService.Infrastructure.Identity;
using AuthService.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AuthService.UnitTests.Services
{
    public class TokenServiceTests
    {
        private readonly JwtSecurityTokenHandler _handler;

        public TokenServiceTests()
        {
            _handler = new();
        }

        private TokenService CreateTokenService(JwtSettings? jwtSettings = null)
        {
            var defaultJwtSettings = new JwtSettings
            {
                Issuer = "AuthService",
                Audience = "AuthServiceClient",
                Key = "THIS_IS_A_TEST_KEY_FOR_UNIT_TESTS_123!",
                DurationInMinutes = 60,
            };

            return new TokenService(Options.Create(jwtSettings ?? defaultJwtSettings));
        }

        private UserInfoDto CreateUserInfoDto(IList<string> roles)
        {
            return new UserInfoDto
            {
                Id = "1",
                FullName = "test-full-name",
                UserName = "test-user-name",
                Email = "user@test.com",
                Roles = roles
            };
        }

        [Fact]
        public void GenerateToken_Should_Return_Valid_Jwt_With_Expected_Claims()
        {
            // Arrange
            var tokenService = CreateTokenService();
            var user = CreateUserInfoDto(["Admin"]);

            // Act
            var token = tokenService.GenerateToken(user);
            var jwt = _handler.ReadJwtToken(token);

            // Assert
            jwt.Claims.Should().NotBeEmpty();
            jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Name)?.Value.Should().Be("test-full-name");
            jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.PreferredUsername)?.Value.Should().Be("test-user-name");
            jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)?.Value.Should().Be("user@test.com");
            jwt.Claims.First(x => x.Type == ClaimTypes.Role).Value.Should().Be("Admin");
        }

        [Fact]
        public void GenerateToken_Should_Set_Expiration_Correctly()
        {
            // Arrange
            var tokenService = CreateTokenService();
            var user = CreateUserInfoDto([]);

            // Act
            var token = tokenService.GenerateToken(user);
            var jwt = _handler.ReadJwtToken(token);

            // Assert
            jwt.ValidTo.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(60), TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void GenerateToken_Should_Work_With_No_Roles()
        {
            // Arrange
            var tokenService = CreateTokenService();
            var user = CreateUserInfoDto([]);

            // Act
            var token = tokenService.GenerateToken(user);
            var jwt = _handler.ReadJwtToken(token);

            // Assert
            jwt.Claims.Any(c => c.Type == ClaimTypes.Role).Should().BeFalse();
        }

    }
}
