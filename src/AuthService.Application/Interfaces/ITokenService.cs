using AuthService.Application.DTOs;
namespace AuthService.Application.Interfaces
{
    /// <summary>
    /// Token Service Interface
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Generates a token with the user and roles
        /// </summary>
        /// <param name="user"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        string GenerateToken(UserInfoDTO user, IList<string> roles);
    }
}
