using DynamicWorkflow.Core.Entities;
using DynamicWorkflow.Core.Entities.Users;
using DynamicWorkflow.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DynamicWorkflow.Services.Services
{
    public class AccountService(ILoggingService<string> _logger,UserManager<ApplicationUser> _userManager, IConfiguration _configuration, ITokenService _tokenService) : IAccountService
    {
        public async Task<(bool isSuccess, string token, string message, bool isDeletionCancelled)> LoginUserAsync(LoginModel model)
        {
            _logger.LogInfo(nameof(AccountService), "LoginUser called");

            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                _logger.LogWarning(nameof(AccountService), "LoginUser, account does not exist");
                return (false, null, "Account does not exist.", false);
            }

            if (await _userManager.IsLockedOutAsync(user))
            {
                _logger.LogWarning(nameof(AccountService), "LoginUser, account locked");
                return (false, null, "Account locked.", false);
            }

            var (canLogin, _, deletionMsg, isDeletionCancelled) = await HandlePendingDeletion(user);
            if (!canLogin)
                return (false, null, deletionMsg, isDeletionCancelled);

            if (!await _userManager.CheckPasswordAsync(user, model.Password))
            {
                _logger.LogWarning(nameof(AccountService), "LoginUser, invalid credentials");
                return (false, null, "Invalid credentials.", false);
            }

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "User";

            var token = _tokenService.CreateToken(user.Id, user.UserName, role);
            var message = isDeletionCancelled ? "Login successful. Deletion request canceled." : "Login successful.";

            _logger.LogInfo(nameof(AccountService), "LoginUser succeeded");
            return (true, token, message, isDeletionCancelled);
        }

        private async Task<(bool isSuccess, string token, string message, bool isDeletionCancelled)> HandlePendingDeletion(ApplicationUser user)
        {
            if (!user.IsPendingDeletion || user.DeletionRequestedAt is null)
                return (true, null, "", false);

            var deletionDays = (DateTime.UtcNow - user.DeletionRequestedAt.Value).TotalDays;
            if (deletionDays < 14)
            {
                user.IsPendingDeletion = false;
                user.DeletionRequestedAt = null;
                await _userManager.UpdateAsync(user);

                _logger.LogInfo(nameof(AccountService), "LoginUser, Deletion request canceled");
                return (true, null, "Deletion request canceled.", true);
            }

            _logger.LogWarning(nameof(AccountService), "LoginUser, account pending deletion period expired");
            return (false, null, "Account has been deleted.", false);
        }
    }
}
