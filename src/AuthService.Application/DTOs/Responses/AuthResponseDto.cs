namespace AuthService.Application.DTOs.Responses
{
    /// <summary>
    /// Auth Response DTO
    /// </summary>
    public class AuthResponseDto
    {
        public string AccessToken { get; init; } = string.Empty;
        public DateTime ExpiresAt { get; init; }
        public string? RefreshToken { get; init; }
    }
}
