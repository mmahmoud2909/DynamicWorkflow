using AutoMapper;
using DynamicWorkflow.Core.DTOs.User;
using DynamicWorkflow.Core.Entities;
using DynamicWorkflow.Core.Entities.Users;
using DynamicWorkflow.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DynamicWorkflow.APIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController(IMapper _mapper,
        SignInManager<ApplicationUser> _signInManager,
        IAccountService _accountService) : ApiController
    {
        // POST : api/account/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterDto model)
        {
            var registerModel = _mapper.Map<RegisterModel>(model);
            var (isSuccess, message, token) = await _accountService.RegisterUserAsync(registerModel);
            if (!isSuccess)
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = message });
            return Ok(new { Status = "Success", Message = message, Token = token });
        }

        // POST : api/account/login
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

        // POST : api/account/logout
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { result = "Logged out" });
        }
    }
}
