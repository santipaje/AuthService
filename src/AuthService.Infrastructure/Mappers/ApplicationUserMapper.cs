using AuthService.Application.DTOs;
using AuthService.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.Mappers
{
    /// <summary>
    /// User Mapper
    /// </summary>
    public static class ApplicationUserMapper
    {
        /// <summary>
        /// Maps from ApplicationUser to UserInfoDTO.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public static UserInfoDto ToUserInfoDto(this ApplicationUser user, IList<string> roles)
        {
            return new UserInfoDto()
            {
                Id = user.Id,
                FullName = user.FullName ?? string.Empty,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Roles = roles.ToList().AsReadOnly(),
            };
        }
    }
}
