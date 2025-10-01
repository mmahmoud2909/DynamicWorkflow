using DynamicWorkflow.Core.Enums;
using DynamicWorkflow.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace DynamicWorkflow.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkflowInstancesController : ControllerBase
    {
        private readonly WorkflowInstanceServices _instanceServices;

        public WorkflowInstancesController(WorkflowInstanceServices instanceServices)
        {
            _instanceServices = instanceServices;
        }

        // User creates a new workflow instance
        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> CreateInstance(int workflowId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var instance = await _instanceServices.CreateInstanceAsync(workflowId, userId);
            return Ok(instance);
        }

        //  Perform action on a workflow step
        [HttpPost("{instanceId}/action")]
        [Authorize]
        public async Task<IActionResult> PerformAction(int instanceId, ActionType action, string? comments = null)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var updatedInstance = await _instanceServices.MoveToNextStepAsync(instanceId, userId, action, comments);
            return Ok(updatedInstance);
        }

        // Get specific instance
        [HttpGet("{instanceId}")]
        public async Task<IActionResult> GetInstance(int instanceId)
        {
            var specificInstance = await _instanceServices.GetInstanceByIdAsync(instanceId);
            if (specificInstance == null)
                return NotFound();

            return Ok(specificInstance);
        }

        //  Get all instances for logged-in user
        [HttpGet("my-instances")]
        [Authorize]
        public async Task<IActionResult> GetAllInstances()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var instances = await _instanceServices.GetInstancesForUserAsync(userId);
            return Ok(instances);
        }
    }
}
