namespace AuthService.Application.DTOs.Responses
{
    /// <summary>
    /// Login Response DTO
    /// </summary>
    public class LoginResponseDto
    {
        public string AccessToken { get; init; } = string.Empty;
        public DateTime ExpiresAt { get; init; }
        public string? RefreshToken { get; init; }
    }
}
