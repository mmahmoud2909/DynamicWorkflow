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
    [Route("api/instance")]
    public class WorkflowInstanceController : ControllerBase
    {
        private readonly WorkflowInstanceService _instanceService;
        private readonly ApplicationIdentityDbContext _context;

        public WorkflowInstanceController(WorkflowInstanceService instanceService, ApplicationIdentityDbContext context)
        {
            _instanceService = instanceService;
            _context = context;
        }

        // 🟢 Create workflow instance (any employee can)
        [HttpPost("create/{workflowId}")]
        [Authorize]
        public async Task<IActionResult> CreateInstance(int workflowId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId);
            if (user == null) return Unauthorized("User not found.");

            var instance = await _instanceService.CreateInstanceAsync(workflowId, user);

            var nextStep = instance.Workflow.Steps
                .OrderBy(s => s.Order)
                .SkipWhile(s => s.Id != instance.CurrentStepId)
                .Skip(1)
                .FirstOrDefault();

            return Ok(new
            {
                message = "✅ Workflow instance created successfully.",
                instanceId = instance.Id,
                workflow = instance.Workflow.Name,
                currentStepId = instance.CurrentStepId,
                currentStep = instance.CurrentStep?.Name,
                nextStep = nextStep?.Name,
                assignedRole = instance.CurrentStep?.AssignedRole.ToString(),
                state = instance.State.ToString(),
                steps = instance.Workflow.Steps.Select(s => new
                {
                    s.Id,
                    s.Name,
                    Status = s.stepStatus.ToString(),
                    s.AssignedRole
                })
            });
        }

        // 🟡 Perform Accept / Reject
        [HttpPost("{instanceId}/action")]
        [Authorize]
        public async Task<IActionResult> MakeAction(int instanceId, [FromQuery] ActionType action)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId);
            if (user == null) return Unauthorized("User not found.");

            var updatedInstance = await _instanceService.MakeActionAsync(instanceId, action, user);

            var nextStep = updatedInstance.Workflow.Steps
                .OrderBy(s => s.Order)
                .SkipWhile(s => s.Id != updatedInstance.CurrentStepId)
                .Skip(1)
                .FirstOrDefault();

            return Ok(new
            {
                message = $"✅ Action '{action}' applied successfully.",
                instanceId = updatedInstance.Id,
                currentStepId = updatedInstance.CurrentStepId,
                currentStep = updatedInstance.CurrentStep?.Name,
                nextStep = nextStep?.Name,
                assignedRole = updatedInstance.CurrentStep?.AssignedRole.ToString(),
                state = updatedInstance.State.ToString(),
                steps = updatedInstance.Workflow.Steps.Select(s => new
                {
                    s.Id,
                    s.Name,
                    Status = s.stepStatus.ToString(),
                    s.AssignedRole
                })
            });
        }

        // 🟣 Get instance details
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetInstance(int id)
        {
            var instance = await _instanceService.GetByIdAsync(id);
            if (instance == null) return NotFound("Instance not found.");

            return Ok(new
            {
                instance.Id,
                instance.Workflow.Name,
                instance.State,
                currentStep = instance.CurrentStep?.Name,
                steps = instance.Workflow.Steps.Select(s => new
                {
                    s.Id,
                    s.Name,
                    Status = s.stepStatus.ToString(),
                    s.AssignedRole
                })
            });
        }

        [HttpGet("{instanceId}/history")]
        [Authorize]
        public async Task<IActionResult> GetInstanceHistory(int instanceId)
        {
            var instance = await _instanceServices.GetInstanceByIdAsync(instanceId);

            if (instance == null)
                return NotFound();

            var history = await _Context.WorkFlowInstanceSteps
                .Where(s => s.InstanceId == instanceId)
                .Include(s => s.Step)
                .ToListAsync();

            var actions = await _Context.WorkflowInstancesAction
                .Where(a => a.WorkflowInstanceId == instanceId)
                .ToListAsync();

            return Ok(new { instance, history, actions });
        }
    }
}
