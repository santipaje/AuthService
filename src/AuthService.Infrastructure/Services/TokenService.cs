using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using AuthService.Infrastructure.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.Infrastructure.Services
{
    /// <summary>
    /// Token Service Implementation
    /// </summary>
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;

        public TokenService(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public string GenerateToken(UserInfoDto user)
        {
            // Claims
            var authClaims = new List<Claim>()
            {
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(JwtRegisteredClaimNames.Name, user.FullName),
                new(JwtRegisteredClaimNames.PreferredUsername, user.UserName),
                new (JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            // Add roles
            authClaims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

            // Auto Sign in key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));

            Log.Debug("Generating JWT token for {Id}", user.Id);

            // Token generation
            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                claims: authClaims,
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            Log.Information("JWT token generated successfully for {Id}", user.Id);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
