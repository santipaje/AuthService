
namespace AuthService.Application.DTOs.Responses
{
    /// <summary>
    /// Resgister Response DTO
    /// </summary>
    public class RegisterResponseDto
    {
        public bool Succeeded { get; set; } = false;
        public IReadOnlyCollection<string> Errors { get; set; } = [];
    }
}
