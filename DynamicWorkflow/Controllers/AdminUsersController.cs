using DynamicWorkflow.Core.DTOs.Department;
using DynamicWorkflow.Core.DTOs.User;
using DynamicWorkflow.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DynamicWorkflow.APIs.Controllers
{
    [ApiController]
    [Route("api/admin/users")]
    [Authorize(Policy = "AdminOnly")]
    public class AdminUsersController : ControllerBase
    {
        private readonly IAdminUserService _svc;
        public AdminUsersController(IAdminUserService svc) => _svc = svc;

        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAll() => Ok(await _svc.GetAllUsersAsync());

        [HttpGet("GetUserbyId/{id:guid}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var user = await _svc.GetUserByIdAsync(id);
            return user == null ? NotFound() : Ok(user);
        }

        [HttpPost("CreateUser")]
        public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
        {
            var created = await _svc.CreateUserAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        [HttpPut("UpdateUser/{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserDto dto)
        {
            await _svc.UpdateUserAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("DeleteUser/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _svc.DeleteUserAsync(id);
            return NoContent();
        }

        // Departments CRUD
        [HttpGet("GetAllDepartments")]
        public async Task<IActionResult> GetDepartments() => Ok(await _svc.GetDepartmentsAsync());

        [HttpPost("CreateDepartment")]
        public async Task<IActionResult> CreateDepartment([FromBody] CreateDepartmentDto dto)
        {
            var d = await _svc.CreateDepartmentAsync(dto);
            return CreatedAtAction(nameof(GetDepartments), new { id = d.Id }, d);
        }

        [HttpPut("UpdateDepartment/{id:guid}")]
        public async Task<IActionResult> UpdateDepartment(Guid id, [FromBody] UpdateDepartmentDto dto)
        {
            await _svc.UpdateDepartmentAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("DeleteDepartment/{id:guid}")]
        public async Task<IActionResult> DeleteDepartment(Guid id)
        {
            await _svc.DeleteDepartmentAsync(id);
            return NoContent();
        }
    }
}
