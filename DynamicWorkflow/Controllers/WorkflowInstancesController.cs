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

        [HttpPost("create/{workflowId}")]
        [Authorize]
        public async Task<IActionResult> CreateInstance(int workflowId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId);
            if (user == null)
                return Unauthorized("User not found.");

            var instance = await _instanceService.CreateInstanceAsync(workflowId, user);

            if (instance == null)
                return BadRequest("Failed to create workflow instance");

            var orderedSteps = instance.Workflow?.Steps?.OrderBy(s => s.Order).ToList();
            var nextStep = orderedSteps.SkipWhile(s => s.Id != instance.CurrentStepId).Skip(1).FirstOrDefault();

            return Ok(new
            {
                message = "✅ Workflow instance created successfully.",
                instanceId = instance.Id,
                workflowId = instance.Workflow.Id,
                workflowName = instance.Workflow.Name,
                workflowStatus = instance.WorkflowStatus.Name,

                currentStepId = instance.CurrentStepId,
                currentStepName = instance.CurrentStep?.Name,
                currentStepStatus = instance.CurrentStep?.workflowStatus.Name,
                currentAssignedRole = instance.CurrentStep?.appRole.Name,

                nextStepId = nextStep?.Id,
                nextStepName = nextStep?.Name,
                nextStepStatus = nextStep?.workflowStatus.Name,
                nextAssignedRole = nextStep?.appRole.Name,

                steps = instance.Workflow.Steps.Select(s => new
                {
                    s.Id,
                    s.Name,
                    Status = s.workflowStatus.Name,
                    AssignedRole = s.appRole.Name
                })
            });
        }

        [HttpPost("{instanceId}/action")]
        [Authorize]
        public async Task<IActionResult> MakeAction(int instanceId, [FromQuery] int actionTypeId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId);
            if (user == null)
                return Unauthorized("User not found.");

            var (completedInstance, nextWorkflowInstance) = await _instanceService.MakeActionAsync(instanceId, actionTypeId, user);

            if (nextWorkflowInstance != null)
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

                    instanceId = nextWorkflowInstance.Id,
                    workflowId = nextWorkflowInstance.Workflow.Id,
                    workflowName = nextWorkflowInstance.Workflow.Name,
                    workflowStatus = nextWorkflowInstance.WorkflowStatus.Name,

                    currentStepId = currentStep?.Id,
                    currentStepName = currentStep?.Name,
                    currentStepStatus = currentStep?.workflowStatus.Name,
                    currentAssignedRole = currentStep?.appRole.Name,

                    nextStepId = nextStep?.Id,
                    nextStepName = nextStep?.Name,
                    nextStepStatus = nextStep?.workflowStatus.Name,
                    nextAssignedRole = nextStep?.appRole.Name,

                    previousWorkflow = new
                    {
                        instanceId = completedInstance.Id,
                        workflowId = completedInstance.Workflow.Id,
                        name = completedInstance.Workflow.Name,
                        status = completedInstance.WorkflowStatus.Name
                    },

                    steps = nextWorkflowInstance.Workflow.Steps.Select(s => new
                    {
                        s.Id,
                        s.Name,
                        Status = s.workflowStatus.Name,
                        AssignedRole = s.appRole.Name
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
                message = "✅ Action applied successfully.",
                workflowChained = false,

                instanceId = completedInstance.Id,
                workflowId = completedInstance.Workflow.Id,
                workflowName = completedInstance.Workflow.Name,
                workflowStatus = completedInstance.WorkflowStatus.Name,

                currentStepId = currentStepNormal?.Id,
                currentStepName = currentStepNormal?.Name,
                currentStepStatus = currentStepNormal?.workflowStatus.Name,
                currentAssignedRole = currentStepNormal?.appRole.Name,

                nextStepId = nextStepNormal?.Id,
                nextStepName = nextStepNormal?.Name,
                nextStepStatus = nextStepNormal?.workflowStatus.Name,
                nextAssignedRole = nextStepNormal?.appRole.Name,

                steps = completedInstance.Workflow.Steps.Select(s => new
                {
                    s.Id,
                    s.Name,
                    Status = s.workflowStatus.Name,
                    AssignedRole = s.appRole.Name
                })
            });
        }

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
                workflowStatus = instance.WorkflowStatus.Name,

                currentStepId = currentStep?.Id,
                currentStepName = currentStep?.Name,
                currentStepStatus = currentStep?.workflowStatus.Name,
                currentAssignedRole = currentStep?.appRole.Name,

                nextStepId = nextStep?.Id,
                nextStepName = nextStep?.Name,
                nextStepStatus = nextStep?.workflowStatus.Name,
                nextAssignedRole = nextStep?.appRole.Name,

                steps = instance.Workflow.Steps.Select(s => new
                {
                    s.Id,
                    s.Name,
                    Status = s.workflowStatus.Name,
                    AssignedRole = s.appRole.Name
                })
            });
        }

        //[HttpGet("chain/{parentWorkflowId}/active")]
        //[Authorize]
        //public async Task<IActionResult> GetActiveWorkflowInChain(int parentWorkflowId)
        //{
        //    var instance = await _instanceService.GetActiveInstanceInChainAsync(parentWorkflowId);
        //    if (instance == null)
        //        return NotFound("No active workflow found in chain.");

        //    var orderedSteps = instance.Workflow.Steps.OrderBy(s => s.Order).ToList();
        //    var currentStep = instance.CurrentStep;
        //    var currentIndex = orderedSteps.FindIndex(s => s.Id == instance.CurrentStepId);
        //    var nextStep = currentIndex >= 0 && currentIndex < orderedSteps.Count - 1
        //        ? orderedSteps[currentIndex + 1]
        //        : null;

        //    return Ok(new
        //    {
        //        instanceId = instance.Id,
        //        workflowId = instance.Workflow.Id,
        //        workflowName = instance.Workflow.Name,
        //        workflowStatus = instance.WorkflowStatus.Name,
        //        currentStepId = currentStep?.Id,
        //        currentStepName = currentStep?.Name,
        //        currentStepStatus = currentStep?.workflowStatus.Name,
        //        currentAssignedRole = currentStep?.appRole.Name,
        //        nextStepId = nextStep?.Id,
        //        nextStepName = nextStep?.Name,
        //        nextStepStatus = nextStep?.workflowStatus.Name,
        //        nextAssignedRole = nextStep?.appRole.Name,
        //        steps = instance.Workflow.Steps.Select(s => new
        //        {
        //            s.Id,
        //            s.Name,
        //            Status = s.workflowStatus.Name,
        //            AssignedRole = s.appRole.Name
        //        })
        //    });
        //}
    
    }
}