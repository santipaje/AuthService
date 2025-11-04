
namespace AuthService.Application.DTOs
{
    /// <summary>
    /// User info DTO
    /// </summary>
    public class UserInfoDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public IList<string> Roles { get; set; } = [];
    }
}
