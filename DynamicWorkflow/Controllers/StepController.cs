using DynamicWorkflow.Core.DTOs.StepDto;
using DynamicWorkflow.Core.Interfaces;
using DynamicWorkflow.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DynamicWorkflow.APIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StepController : ControllerBase
    {
        private readonly IStepService _stepService;
        private readonly ApplicationIdentityDbContext _context;
        
        private readonly IAdminWorkflowService _svc;

        public StepController(IStepService stepService, ApplicationIdentityDbContext context, IAdminWorkflowService svc)
        {
            _stepService = stepService;
            _context = context;
            _svc = svc;
        }

        [HttpGet("{workflowId}/getallsteps")]
        public async Task<IActionResult> GetAllSteps(int workflowId)
        {
            try
            {
                var steps = await _stepService.GetAllStepsAsync(workflowId);
                return Ok(steps);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("steps/{stepId}")]
        public async Task<IActionResult> GetStepById(int stepId)
        {
            try
            {
                var step = await _stepService.GetStepByIdAsync(stepId);
                if (step == null)
                    return NotFound($"Step with ID {stepId} not found.");

                return Ok(step);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost("Admin/AddStep/{workflowId:int}")]
        public async Task<IActionResult> AddStep(int workflowId, [FromBody] CreateStepDto dto)
        {
            var stepId = await _svc.AddStepAsync(workflowId, dto);
            return Created("", new { stepId });
        }
        [NonAction]
        public async Task<IActionResult> Get(int id)
        {
            var wf = await _svc.GetWorkflowByIdAsync(id);
            return wf == null ? NotFound() : Ok(wf);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPut("Admin/UpdateStep/{stepId:int}")]
        public async Task<IActionResult> UpdateStep(int stepId, [FromBody] UpdateStepDto dto)
        {
            await _svc.UpdateStepAsync(stepId, dto);
            return NoContent();
        }

        [HttpDelete("Admin/DeleteStep/{stepId:int}")]
        public async Task<IActionResult> DeleteStep(int stepId)
        {
            await _svc.DeleteStepAsync(stepId);
            return NoContent();
        }

        private async Task<List<string>> GetUserRoles(Guid userId)
        {
            return await (from ur in _context.UserRoles
                          join r in _context.Roles on ur.RoleId equals r.Id
                          where ur.UserId == userId
                          select r.Name).ToListAsync();
        }
    }
}