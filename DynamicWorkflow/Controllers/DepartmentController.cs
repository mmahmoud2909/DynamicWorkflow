using DynamicWorkflow.Core.DTOs.Department;
using DynamicWorkflow.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DynamicWorkflow.APIs.Controllers
{
    [Route("api/admin")]
    [Authorize(Policy = "AdminOnly")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IAdminUserService _svc;
        public DepartmentController(IAdminUserService svc) => _svc = svc;

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
