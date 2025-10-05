using DynamicWorkflow.Core.Enums;
using DynamicWorkflow.Core.Interfaces;
using DynamicWorkflow.Infrastructure.Identity;
using DynamicWorkflow.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
namespace DynamicWorkflow.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkflowInstancesController : ControllerBase
    {
        private readonly IworkflowInstanceService _instanceServices;
        private readonly ApplicationIdentityDbContext _Context;

        public WorkflowInstancesController(IworkflowInstanceService instanceServices,ApplicationIdentityDbContext Context)
        {
            _instanceServices = instanceServices;
            _Context = Context;
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
        // ✅ Get instance with history (steps + actions)
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
