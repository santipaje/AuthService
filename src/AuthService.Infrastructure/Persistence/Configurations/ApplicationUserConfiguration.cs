using AuthService.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// ApplicationUser configuration
    /// </summary>
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(u => u.UserName)
                .HasMaxLength(100);

            builder.Property(u => u.CreatedTime)
                .HasDefaultValueSql("GETUTCDATE()");

        }
    }
}
