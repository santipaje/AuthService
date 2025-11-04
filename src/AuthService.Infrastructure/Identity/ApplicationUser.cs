using Microsoft.AspNetCore.Identity;

namespace AuthService.Infrastructure.Identity
{
    /// <summary>
    /// ApplicationUser which extends IdentityUser, a widely used ASP.NET Core framework that provides auth management.
    /// Metadata can be added.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        public string? Name { get; set; }
        public DateTime CreatedTime { get; set; } = DateTime.Now;
    }
}
