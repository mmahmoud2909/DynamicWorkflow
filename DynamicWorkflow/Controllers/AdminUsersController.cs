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
    public class AdminUsersController : ControllerBase
    {
        private readonly IAdminUserService _svc;
        public AdminUsersController(IAdminUserService svc) => _svc = svc;

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
        [HttpGet("GetAllDepartments")]
        public async Task<IActionResult> GetAllDepartments()
        {
            var departments = await _svc.GetDepartmentsAsync();

            if (!departments.Any())
                return Ok(new { message = "No departments found." });

            return Ok(new
            {
                message = "✅ Departments retrieved successfully.",
                totalCount = departments.Count,
                departments
            });
        }
        [HttpGet("GetDepartmentById/{id:guid}")]
        public async Task<IActionResult> GetDepartmentById(Guid id)
        {
            var department = await _svc.GetDepartmentByIdAsync(id);
            if (department == null)
                return NotFound(new { message = "❌ Department not found." });

            return Ok(new
            {
                message = "✅ Department retrieved successfully.",
                department
            });
        }

        [HttpPost("CreateDepartments")]
        public async Task<IActionResult> CreateDepartment([FromBody] CreateDepartmentDto dto)
        {
            var d = await _svc.CreateDepartmentAsync(dto);
            return CreatedAtAction(nameof(GetDepartmentById), new { id = d.Id }, new
            {
                message = "✅ Department created successfully.",
                department = new { d.Id, d.Name }
            });
        }

        [HttpPut("UpdateDepartments/{id:guid}")]
        public async Task<IActionResult> UpdateDepartment(Guid id, [FromBody] UpdateDepartmentDto dto)
        {
            await _svc.UpdateDepartmentAsync(id, dto);
            return Ok(new
            {
                message = "✅ Department updated successfully.",
                updatedDepartmentId = id
            });
        }

        [HttpDelete("DeleteDepartment/{id:guid}")]
        public async Task<IActionResult> DeleteDepartment(Guid id)
        {
            await _svc.DeleteDepartmentAsync(id);
            return Ok(new { message = $"🗑️ Department with ID {id} deleted successfully." });
        }
    }
}
