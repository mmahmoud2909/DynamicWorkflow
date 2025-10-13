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

            var orderedSteps = instance.Workflow.Steps.OrderBy(s => s.Order).ToList();
            var nextStep = orderedSteps.SkipWhile(s => s.Id != instance.CurrentStepId).Skip(1).FirstOrDefault();

            return Ok(new
            {
                message = "✅ Workflow instance created successfully.",
                instanceId = instance.Id,
                workflowId = instance.Workflow.Id,
                workflowName = instance.Workflow.Name,
                state = instance.State.ToString(),

                currentStepId = instance.CurrentStepId,
                currentStepName = instance.CurrentStep?.Name,
                currentStepStatus = instance.CurrentStep?.stepStatus.ToString(),
                currentAssignedRole = instance.CurrentStep?.AssignedRole.ToString(),

                nextStepId = nextStep?.Id,
                nextStepName = nextStep?.Name,
                nextStepStatus = nextStep?.stepStatus.ToString(),
                nextAssignedRole = nextStep?.AssignedRole.ToString(),

                steps = instance.Workflow.Steps.Select(s => new
                {
                    s.Id,
                    s.Name,
                    Status = s.stepStatus.ToString(),
                    AssignedRole = s.AssignedRole.ToString()
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
            if (user == null)
                return Unauthorized("User not found.");

            // ✅ هنا بترجع (الانستانس بعد التعديل + الانستانس بتاع الوركفلو اللي بعده)
            var (updatedInstance, nextWorkflowInstance) = await _instanceService.MakeActionAsync(instanceId, action, user);

            var orderedSteps = updatedInstance.Workflow.Steps.OrderBy(s => s.Order).ToList();
            var currentStep = updatedInstance.CurrentStep;
            var currentIndex = orderedSteps.FindIndex(s => s.Id == updatedInstance.CurrentStepId);
            var nextStep = currentIndex >= 0 && currentIndex < orderedSteps.Count - 1
                ? orderedSteps[currentIndex + 1]
                : null;

            return Ok(new
            {
                message = $"✅ Action '{action}' applied successfully.",

                // 🔹 Current Step Info
                currentStepId = currentStep?.Id,
                currentStepName = currentStep?.Name,
                currentStepStatus = currentStep?.stepStatus.ToString(),
                currentAssignedRole = currentStep?.AssignedRole.ToString(),

                // 🔸 Next Step Info
                nextStepId = nextStep?.Id,
                nextStepName = nextStep?.Name,
                nextStepStatus = nextStep?.stepStatus.ToString(),
                nextAssignedRole = nextStep?.AssignedRole.ToString(),

                // 🔹 Workflow Info
                workflowId = updatedInstance.Workflow.Id,
                workflowName = updatedInstance.Workflow.Name,
                workflowState = updatedInstance.State.ToString(),

                // ⚡ If workflow completed and next workflow started
                nextWorkflow = nextWorkflowInstance == null ? null : new
                {
                    id = nextWorkflowInstance.Workflow.Id,
                    name = nextWorkflowInstance.Workflow.Name,
                    state = nextWorkflowInstance.State.ToString(),
                    currentStep = nextWorkflowInstance.CurrentStep?.Name,
                    assignedRole = nextWorkflowInstance.CurrentStep?.AssignedRole.ToString()
                },

                // 📋 All steps summary
                steps = updatedInstance.Workflow.Steps.Select(s => new
                {
                    s.Id,
                    s.Name,
                    Status = s.stepStatus.ToString(),
                    AssignedRole = s.AssignedRole.ToString()
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

            var orderedSteps = instance.Workflow.Steps.OrderBy(s => s.Order).ToList();
            var currentStep = instance.CurrentStep;
            var currentIndex = orderedSteps.FindIndex(s => s.Id == instance.CurrentStepId);
            var nextStep = currentIndex >= 0 && currentIndex < orderedSteps.Count - 1
                ? orderedSteps[currentIndex + 1]
                : null;

            return Ok(new
            {
                instanceId = instance.Id,
                workflowId = instance.Workflow.Id,
                workflowName = instance.Workflow.Name,
                workflowState = instance.State.ToString(),

                currentStepId = currentStep?.Id,
                currentStepName = currentStep?.Name,
                currentStepStatus = currentStep?.stepStatus.ToString(),
                currentAssignedRole = currentStep?.AssignedRole.ToString(),

                nextStepId = nextStep?.Id,
                nextStepName = nextStep?.Name,
                nextStepStatus = nextStep?.stepStatus.ToString(),
                nextAssignedRole = nextStep?.AssignedRole.ToString(),

                steps = instance.Workflow.Steps.Select(s => new
                {
                    s.Id,
                    s.Name,
                    Status = s.stepStatus.ToString(),
                    AssignedRole = s.AssignedRole.ToString()
                })
            });
        }
    }
}
