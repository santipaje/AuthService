using AuthService.Application.DTOs.Requests;
using AuthService.Application.DTOs.Responses;

namespace AuthService.Application.Interfaces
{
    public interface IAuthService
    {
        /// <summary>
        /// Registers a user in the system
        /// </summary>
        /// <param name="registerDto"></param>
        /// <returns></returns>
        Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto registerDto);

        /// <summary>
        /// Logs in a user and returns a response with its access token
        /// </summary>
        /// <param name="loginDto"></param>
        /// <returns></returns>
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginDto);
    }
}
