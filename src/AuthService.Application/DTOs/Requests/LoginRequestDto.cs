namespace AuthService.Application.DTOs.Requests
{
    /// <summary>
    /// Login DTO
    /// </summary>
    public class LoginRequestDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
