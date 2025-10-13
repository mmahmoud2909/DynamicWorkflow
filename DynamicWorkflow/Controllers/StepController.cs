using DynamicWorkflow.Core.Entities.Users;
using DynamicWorkflow.Core.Enums;
using DynamicWorkflow.Infrastructure.Identity;
using DynamicWorkflow.Services.Services;
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
        private readonly StepService _stepService;
        private readonly ApplicationIdentityDbContext _context;

        public StepController(StepService stepService, ApplicationIdentityDbContext context)
        {
            _stepService = stepService;
            _context = context;
        }

        /// <summary>
        /// Get all steps of a workflow (visible to Admins or users in any step’s role).
        /// </summary>
        [HttpGet("{workflowId}/steps")]
        public async Task<IActionResult> GetAllSteps(int workflowId)
        {
            var steps = await _stepService.GetAllStepsAsync(workflowId);
            return Ok(steps);
        }

        /// <summary>
        /// Perform an action (Accept, Reject, etc.) on a step.
        /// Authorization is based on the AssignedRole of the step.
        /// </summary>
        [HttpPost("{workflowId}/step/{stepId}/action")]
        public async Task<IActionResult> PerformAction(int workflowId, int stepId, [FromQuery] ActionType action)
        {
            // ✅ Identify the logged-in user
            var email = User.FindFirstValue(ClaimTypes.Email)
                       ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (currentUser == null)
                return Unauthorized("User not found or not authenticated.");

            // ✅ Load workflow with its steps
            var workflow = await _context.Workflows
                .Include(w => w.Steps)
                .FirstOrDefaultAsync(w => w.Id == workflowId);

            if (workflow == null)
                return NotFound($"Workflow with ID {workflowId} not found.");

            // ✅ Perform the action
            await _stepService.MakeActionAsync(workflow, stepId, action, currentUser);

            return Ok(new
            {
                message = $"Action '{action}' completed successfully",
                workflowId,
                stepId,
                performedBy = currentUser.DisplayName,
                status = workflow.Steps.First(s => s.Id == stepId).stepStatus
            });
        }
    }
}
