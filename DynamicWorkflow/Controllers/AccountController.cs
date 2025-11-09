using AutoMapper;
using DynamicWorkflow.Core.DTOs.User;
using DynamicWorkflow.Core.Entities;
using DynamicWorkflow.Core.Entities.Users;
using DynamicWorkflow.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DynamicWorkflow.APIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController(IMapper _mapper,
        SignInManager<ApplicationUser> _signInManager,
        IAccountService _accountService) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var loginModel = _mapper.Map<LoginModel>(model);
            var (isSuccess, token, message, isDeletionCancelled) = await _accountService.LoginUserAsync(loginModel);

            if (!isSuccess)
                return Unauthorized(new { Message = message });

            return Ok(new
            {
                Token = token,
                Message = isDeletionCancelled ? "Login successful. Deletion request canceled." : message
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { result = "Logged out" });
        }
    }
}
