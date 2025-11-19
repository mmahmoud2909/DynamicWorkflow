using DynamicWorkflow.Core.DTOs.Department;
using DynamicWorkflow.Core.DTOs.User;
using DynamicWorkflow.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DynamicWorkflow.APIs.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Policy = "AdminOnly")]
    public class UsersController : ControllerBase
    {
        private readonly IAdminUserService _svc;
        public UsersController(IAdminUserService svc) => _svc = svc;

        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _svc.GetAllUsersAsync();

            if (users == null || !users.Any())
                return Ok(new { message = "No users found." });

            return Ok(new
            {
                message = "✅ All registered users retrieved successfully.",
                totalCount = users.Count,
                users
            });
        }

        [HttpGet("GetUserById/{id:guid}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await _svc.GetUserByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "❌ User not found." });

            return Ok(new
            {
                message = "✅ User retrieved successfully.",
                user
            });
        }

        [HttpPost("CreateUsers")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Invalid data provided.", errors = ModelState });

            var created = await _svc.CreateUserAsync(dto);

            return CreatedAtAction(nameof(GetUserById), new { id = created.Id }, new
            {
                message = "✅ User created successfully.",
                user = new
                {
                    created.Id,
                    created.DisplayName,
                    created.Email,
                    created.DepartmentId,
                    created.ManagerId,
                    created.RegisteredAt,
                    created.IsPendingDeletion,
                    created.ProfilePicUrl,
                    Role = dto.Role.ToString()
                }
            });
        }

        [HttpPut("UpdateUsers/{id:guid}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserDto dto)
        {
            await _svc.UpdateUserAsync(id, dto);

            return Ok(new
            {
                message = "✅ User updated successfully.",
                updatedUserId = id
            });
        }

        [HttpDelete("DeleteUsers/{id:guid}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            await _svc.DeleteUserAsync(id);
            return Ok(new { message = $"🗑️ User with ID {id} deleted successfully." });
        }
    }
}
