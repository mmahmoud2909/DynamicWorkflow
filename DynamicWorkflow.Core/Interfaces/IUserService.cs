using DynamicWorkflow.Core.DTOs.User;
using DynamicWorkflow.Core.Entities.Users;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWorkflow.Core.Interfaces
{
    public interface IUserService
    {
        Task<ApplicationUser> GetCurrentUserAsync();
        Task<List<UserDto>> GetAllUsersAsync();
        Task<IdentityResult> UpdateUserAsync(EditUserDto editUserDto);
        Task<IdentityResult> RequestAccountDeletionAsync(string userId);

    }
}
