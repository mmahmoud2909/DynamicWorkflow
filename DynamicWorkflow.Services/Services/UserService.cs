using AutoMapper;
using DynamicWorkflow.Core.DTOs.User;
using DynamicWorkflow.Core.Entities.Users;
using DynamicWorkflow.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DynamicWorkflow.Services.Services
{
    public class UserService(IHttpContextAccessor _httpContextAccessor, ILoggingService<string> _logger,
    IMapper _mapper, UserManager<ApplicationUser> _userManager) : IUserService
    {
        public async Task<ApplicationUser> GetCurrentUserAsync()
        {
            _logger.LogInfo(nameof(UserService), "GetCurrentUser called");

            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("No user is logged in.");

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == userId);

            if (user == null)
                throw new InvalidOperationException("The user was not found.");

            _logger.LogInfo(nameof(UserService), "GetCurrentUser succeeded");
            return user;
        }

        public async Task<IdentityResult> UpdateUserAsync(EditUserDto dto)
        {
            _logger.LogInfo(nameof(UserService), "UpdateUser called");
            var user = await GetCurrentUserAsync();

            var validationResult = await ValidateAndUpdateUserFields(dto, user);
            if (!validationResult.Succeeded)
                return validationResult;

            //await UpdateUserImageAsync(dto, user);

            var result = await _userManager.UpdateAsync(user);
            _logger.LogInfo(nameof(UserService), result.Succeeded ? "UpdateUser succeeded" : "UpdateUser failed");

            return result;
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            _logger.LogInfo(nameof(UserService), "GetAllUsersAsync called");

            try
            {
                var users = await _userManager.GetUsersInRoleAsync("User");
                return users?.Select(MapUserToDto).ToList() ?? new List<UserDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(nameof(UserService), "Error while getting users", ex);
                return new List<UserDto>();
            }
        }

        public async Task<IdentityResult> RequestAccountDeletionAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });

            user.IsPendingDeletion = true;
            user.DeletionRequestedAt = DateTime.UtcNow;

            return await _userManager.UpdateAsync(user);
        }
        private UserDto MapUserToDto(ApplicationUser user)
        {
            return new()
            {
                Id =user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PasswordHash = user.PasswordHash,
                ProfilePicUrl = user.ProfilePicUrl,
                IsLockedOut = user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow
            };
        }

        private async Task<IdentityResult> ValidateAndUpdateUserFields(EditUserDto dto, ApplicationUser user)
        {
            if (!string.IsNullOrWhiteSpace(dto.Username) && dto.Username != user.UserName)
            {
                if (await _userManager.FindByNameAsync(dto.Username) != null)
                    return IdentityResult.Failed(new IdentityError { Description = "Username already exists." });

                user.UserName = dto.Username;
            }

            if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email != user.Email)
            {
                if (await _userManager.FindByEmailAsync(dto.Email) != null)
                    return IdentityResult.Failed(new IdentityError { Description = "Email already exists." });

                user.Email = dto.Email;
            }

            if (!string.IsNullOrWhiteSpace(dto.NewPassword))
            {
                if (string.IsNullOrWhiteSpace(dto.OldPassword))
                    return IdentityResult.Failed(new IdentityError { Description = "Old password is required." });

                if (!await _userManager.CheckPasswordAsync(user, dto.OldPassword))
                    return IdentityResult.Failed(new IdentityError { Description = "Old password is incorrect." });

                if (dto.NewPassword != dto.ConfirmNewPassword)
                    return IdentityResult.Failed(new IdentityError { Description = "Password confirmation does not match." });

                user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, dto.NewPassword);
            }

            return IdentityResult.Success;
        }

        Task<Microsoft.AspNetCore.Identity.IdentityResult> IUserService.UpdateUserAsync(EditUserDto editUserDto)
        {
            throw new NotImplementedException();
        }

        Task<Microsoft.AspNetCore.Identity.IdentityResult> IUserService.RequestAccountDeletionAsync(string userId)
        {
            throw new NotImplementedException();
        }
    }
}
