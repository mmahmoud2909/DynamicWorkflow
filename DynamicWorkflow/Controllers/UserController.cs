using AutoMapper;
using DynamicWorkflow.Core.DTOs.User;
using DynamicWorkflow.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DynamicWorkflow.APIs.Controllers
{
    namespace News.API.Controllers
    {
        [Authorize(Roles = "User")]
        public class UserController(IUserService _userService, IMapper _mapper) : ApiController
        {
            [HttpGet("myProfile")]
            public async Task<IActionResult> GetCurrentUserInfo()
            {
                var user = await _userService.GetCurrentUserAsync();
                var userInfo = _mapper.Map<UserDto>(user);
                return Ok(userInfo);
            }

            [HttpPut("editMyProfile")]
            public async Task<IActionResult> EditUserInfo([FromForm] EditUserDto model)
            {
                var result = await _userService.UpdateUserAsync(model);
                if (result.Succeeded)
                    return Ok(new { result = "User updated successfully" });

                return BadRequest(result.Errors);
            }
        }
    }
}
