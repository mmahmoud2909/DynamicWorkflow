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

        // Create workflow instance       
        [HttpPost("create/{workflowId}")]
        [Authorize]
        public async Task<IActionResult> CreateInstance(int workflowId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId);
            if (user == null)
                return Unauthorized("User not found.");

            var instance = await _instanceService.CreateInstanceAsync(workflowId, user);

            var orderedSteps = instance.Workflow.Steps.OrderBy(s => s.Order).ToList();
            var nextStep = orderedSteps.SkipWhile(s => s.Id != instance.CurrentStepId).Skip(1).FirstOrDefault();

            return Ok(new
            {
                message = "✅ Workflow instance created successfully.",
                instanceId = instance.Id,
                workflowId = instance.Workflow.Id,
                workflowName = instance.Workflow.Name,
                instanceState = instance.State.ToString(),

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
                    InstanceState = s.stepStatus switch
                    {
                        Status.Accepted => Status.Accepted.ToString(),
                        Status.Rejected => Status.Rejected.ToString(),
                        Status.InProgress => Status.InProgress.ToString(),
                        _ => instance.State.ToString()
                    },
                    AssignedRole = s.AssignedRole.ToString()
                })
            });
        }

        // 🟡 Perform action (Accept / Reject)
        [HttpPost("{instanceId}/action")]
        [Authorize]
        public async Task<IActionResult> MakeAction(int instanceId, [FromQuery] ActionType action)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId);
            if (user == null)
                return Unauthorized("User not found.");

            var (completedInstance, nextWorkflowInstance) = await _instanceService.MakeActionAsync(instanceId, action, user);

            // If workflow completed and chained to next workflow, return the NEW workflow as active
            if (nextWorkflowInstance != null && completedInstance.State == Status.Completed)
            {
                var orderedSteps = nextWorkflowInstance.Workflow.Steps.OrderBy(s => s.Order).ToList();
                var currentStep = nextWorkflowInstance.CurrentStep;
                var currentIndex = orderedSteps.FindIndex(s => s.Id == nextWorkflowInstance.CurrentStepId);
                var nextStep = currentIndex >= 0 && currentIndex < orderedSteps.Count - 1
                    ? orderedSteps[currentIndex + 1]
                    : null;

                return Ok(new
                {
                    message = $"✅ Workflow '{completedInstance.Workflow.Name}' completed! Moved to '{nextWorkflowInstance.Workflow.Name}'.",
                    workflowChained = true,

                    // NEW ACTIVE INSTANCE INFO
                    instanceId = nextWorkflowInstance.Id,
                    workflowId = nextWorkflowInstance.Workflow.Id,
                    workflowName = nextWorkflowInstance.Workflow.Name,
                    instanceState = nextWorkflowInstance.State.ToString(),

                    // Current Step of NEW workflow
                    currentStepId = currentStep?.Id,
                    currentStepName = currentStep?.Name,
                    currentStepStatus = currentStep?.stepStatus.ToString(),
                    currentAssignedRole = currentStep?.AssignedRole.ToString(),

                    // Next Step of NEW workflow
                    nextStepId = nextStep?.Id,
                    nextStepName = nextStep?.Name,
                    nextStepStatus = nextStep?.stepStatus.ToString(),
                    nextAssignedRole = nextStep?.AssignedRole.ToString(),

                    // Previous completed workflow info
                    previousWorkflow = new
                    {
                        instanceId = completedInstance.Id,
                        workflowId = completedInstance.Workflow.Id,
                        name = completedInstance.Workflow.Name,
                        state = completedInstance.State.ToString()
                    },

                    // Steps of NEW workflow
                    steps = nextWorkflowInstance.Workflow.Steps.Select(s => new
                    {
                        s.Id,
                        s.Name,
                        Status = s.stepStatus.ToString(),
                        InstanceState = s.stepStatus switch
                        {
                            Status.Accepted => Status.Accepted.ToString(),
                            Status.Rejected => Status.Rejected.ToString(),
                            Status.InProgress => Status.InProgress.ToString(),
                            _ => nextWorkflowInstance.State.ToString()
                        },
                        AssignedRole = s.AssignedRole.ToString()
                    })
                });
            }

            // Normal flow - no workflow chaining
            var orderedStepsNormal = completedInstance.Workflow.Steps.OrderBy(s => s.Order).ToList();
            var currentStepNormal = completedInstance.CurrentStep;
            var currentIndexNormal = orderedStepsNormal.FindIndex(s => s.Id == completedInstance.CurrentStepId);
            var nextStepNormal = currentIndexNormal >= 0 && currentIndexNormal < orderedStepsNormal.Count - 1
                ? orderedStepsNormal[currentIndexNormal + 1]
                : null;

            return Ok(new
            {
                message = completedInstance.State == Status.Completed
                    ? "✅ All workflows completed!"
                    : $"✅ Action '{action}' applied successfully.",
                workflowChained = false,

                instanceId = completedInstance.Id,
                workflowId = completedInstance.Workflow.Id,
                workflowName = completedInstance.Workflow.Name,
                instanceState = completedInstance.State.ToString(),

                currentStepId = currentStepNormal?.Id,
                currentStepName = currentStepNormal?.Name,
                currentStepStatus = currentStepNormal?.stepStatus.ToString(),
                currentAssignedRole = currentStepNormal?.AssignedRole.ToString(),

                nextStepId = nextStepNormal?.Id,
                nextStepName = nextStepNormal?.Name,
                nextStepStatus = nextStepNormal?.stepStatus.ToString(),
                nextAssignedRole = nextStepNormal?.AssignedRole.ToString(),

                steps = completedInstance.Workflow.Steps.Select(s => new
                {
                    s.Id,
                    s.Name,
                    Status = s.stepStatus.ToString(),
                    InstanceState = s.stepStatus switch
                    {
                        Status.Accepted => Status.Accepted.ToString(),
                        Status.Rejected => Status.Rejected.ToString(),
                        Status.InProgress => Status.InProgress.ToString(),
                        _ => completedInstance.State.ToString()
                    },
                    AssignedRole = s.AssignedRole.ToString()
                })
            });
        }

        // Get instance by ID
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetInstance(int id)
        {
            var instance = await _instanceService.GetByIdAsync(id);
            if (instance == null)
                return NotFound("Instance not found.");

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
                instanceState = instance.State.ToString(),

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
                    InstanceState = s.stepStatus switch
                    {
                        Status.Accepted => Status.Accepted.ToString(),
                        Status.Rejected => Status.Rejected.ToString(),
                        Status.InProgress => Status.InProgress.ToString(),
                        _ => instance.State.ToString()
                    },
                    AssignedRole = s.AssignedRole.ToString()
                })
            });
        }

        // Get active workflow in chain
        [HttpGet("chain/{parentWorkflowId}/active")]
        [Authorize]
        public async Task<IActionResult> GetActiveWorkflowInChain(int parentWorkflowId)
        {
            var instance = await _instanceService.GetActiveInstanceInChainAsync(parentWorkflowId);
            if (instance == null)
                return NotFound("No active workflow found in chain.");

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
                instanceState = instance.State.ToString(),
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
                    InstanceState = s.stepStatus.ToString(),
                    AssignedRole = s.AssignedRole.ToString()
                })
            });
        }
    }
}