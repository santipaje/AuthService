using AuthService.Application.DTOs;

namespace AuthService.Application.Interfaces
{
    /// <summary>
    /// Token Service Interface
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Generates a JWT token with the user and its roles
        /// </summary>
        /// <param name="user"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        string GenerateToken(UserInfoDto user);
    }
}
