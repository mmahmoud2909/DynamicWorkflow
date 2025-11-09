using DynamicWorkflow.Core.Entities;
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

        public StepController(IStepService stepService, ApplicationIdentityDbContext context)
        {
            _stepService = stepService;
            _context = context;
        }

        [HttpGet("{workflowId}/steps")]
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

        [HttpPost("{workflowId}/step/{stepId}/action")]
        public async Task<IActionResult> PerformAction(int workflowId, int stepId, [FromQuery] int actionTypeId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId);
                if (currentUser == null)
                    return Unauthorized("User not found or not authenticated.");

                var workflow = await _context.Workflows
                    .Include(w => w.Steps)
                    .FirstOrDefaultAsync(w => w.Id == workflowId);

                if (workflow == null)
                    return NotFound($"Workflow with ID {workflowId} not found.");

                await _stepService.MakeActionAsync(workflow, stepId, actionTypeId, currentUser);

                var updatedStep = await _stepService.GetStepByIdAsync(stepId);

                return Ok(new
                {
                    message = "Action completed successfully",
                    workflowId,
                    stepId,
                    actionTypeId,
                    performedBy = currentUser.DisplayName,
                    stepStatus = updatedStep?.workflowStatus?.Name
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("steps/{stepId}/can-perform")]
        public async Task<IActionResult> CanPerformStep(int stepId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId);
                if (currentUser == null)
                    return Unauthorized("User not found or not authenticated.");

                var canPerform = await _stepService.CanUserPerformStepAsync(stepId, currentUser);

                return Ok(new
                {
                    stepId,
                    userId = currentUser.Id,
                    canPerform,
                    userRoles = await GetUserRoles(currentUser.Id)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("steps")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> CreateStep([FromBody] CreateStepRequest request)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var step = new WorkflowStep
                {
                    Name = request.Name,
                    Comments = request.Comments,
                    Order = request.Order,
                    WorkflowStatusId = request.WorkflowStatusId,
                    ActionTypeEntityId = request.ActionTypeEntityId,
                    AppRoleId = request.AppRoleId,
                    isEndStep = request.IsEndStep,
                    WorkflowId = request.WorkflowId,
                    AssignedUserId = request.AssignedUserId
                };

                var createdStep = await _stepService.CreateStepAsync(step, userId);

                return CreatedAtAction(nameof(GetStepById), new { stepId = createdStep.Id }, createdStep);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("steps/{stepId}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateStep(int stepId, [FromBody] UpdateStepRequest request)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var existingStep = await _stepService.GetStepByIdAsync(stepId);

                if (existingStep == null)
                    return NotFound($"Step with ID {stepId} not found.");

                if (!string.IsNullOrEmpty(request.Name))
                    existingStep.Name = request.Name;

                if (request.Comments != null)
                    existingStep.Comments = request.Comments;

                if (request.Order.HasValue)
                    existingStep.Order = request.Order.Value;

                if (request.WorkflowStatusId.HasValue)
                    existingStep.WorkflowStatusId = request.WorkflowStatusId.Value;

                if (request.ActionTypeEntityId.HasValue)
                    existingStep.ActionTypeEntityId = request.ActionTypeEntityId.Value;

                if (request.AppRoleId.HasValue)
                    existingStep.AppRoleId = request.AppRoleId.Value;

                if (request.IsEndStep.HasValue)
                    existingStep.isEndStep = request.IsEndStep.Value;

                if (request.AssignedUserId.HasValue)
                    existingStep.AssignedUserId = request.AssignedUserId.Value;

                await _stepService.UpdateStepAsync(existingStep, userId);

                return Ok(existingStep);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private async Task<List<string>> GetUserRoles(Guid userId)
        {
            return await (from ur in _context.UserRoles
                          join r in _context.Roles on ur.RoleId equals r.Id
                          where ur.UserId == userId
                          select r.Name).ToListAsync();
        }
    }

    public class CreateStepRequest
    {
        public string Name { get; set; }
        public string? Comments { get; set; }
        public int Order { get; set; }
        public int WorkflowStatusId { get; set; }
        public int ActionTypeEntityId { get; set; }
        public int AppRoleId { get; set; }
        public bool IsEndStep { get; set; }
        public int WorkflowId { get; set; }
        public Guid? AssignedUserId { get; set; }
    }

    public class UpdateStepRequest
    {
        public string? Name { get; set; }
        public string? Comments { get; set; }
        public int? Order { get; set; }
        public int? WorkflowStatusId { get; set; }
        public int? ActionTypeEntityId { get; set; }
        public int? AppRoleId { get; set; }
        public bool? IsEndStep { get; set; }
        public Guid? AssignedUserId { get; set; }
    }
}