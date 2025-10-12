using DynamicWorkflow.Core.Entities.Users;
using DynamicWorkflow.Core.Enums;
using DynamicWorkflow.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DynamicWorkflow.APIs.Controllers
{
    [ApiController]
    [Route("api/instance")]
    [Authorize] // ✅ Only authenticated users
    public class WorkflowInstanceController : ControllerBase
    {
        private readonly WorkflowInstanceService _instanceService;
        private readonly UserManager<ApplicationUser> _userManager;

        public WorkflowInstanceController(WorkflowInstanceService instanceService, UserManager<ApplicationUser> userManager)
        {
            _instanceService = instanceService;
            _userManager = userManager;
        }

        // 🟢 1️⃣ Create a new instance (like creating a new PR)
        [HttpPost("create/{workflowId}")]
        public async Task<IActionResult> CreateInstance(int workflowId)
        {
            var email = User.Identity?.Name ?? User.FindFirstValue(ClaimTypes.Name);
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return Unauthorized("User not found or not authenticated.");

            var instance = await _instanceService.CreateInstanceAsync(workflowId, user);

            return Ok(new
            {
                message = "✅ New workflow instance created successfully.",
                instance.Id,
                Workflow = instance.Workflow?.Name,
                CurrentStep = instance.CurrentStep?.Name,
                AssignedRole = instance.CurrentStep?.AssignedRole.ToString(),
                State = instance.State.ToString()
            });
        }

        // 🟡 2️⃣ Perform an action on the current step (e.g., Accept, Hold, Reject)
        [HttpPost("{instanceId}/action")]
        public async Task<IActionResult> MakeAction(int instanceId, [FromQuery] ActionType action)
        {
            var email = User.Identity?.Name ?? User.FindFirstValue(ClaimTypes.Name);
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return Unauthorized("User not found or not authenticated.");

            try
            {
                await _instanceService.MakeActionAsync(instanceId, action, user);
                var updated = await _instanceService.GetByIdAsync(instanceId);

                return Ok(new
                {
                    message = $"✅ Action '{action}' performed successfully.",
                    CurrentStep = updated?.CurrentStep?.Name,
                    CurrentRole = updated?.CurrentStep?.AssignedRole.ToString(),
                    State = updated?.State.ToString()
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 🟣 3️⃣ Get single instance details
        [HttpGet("{instanceId}")]
        public async Task<IActionResult> GetInstance(int instanceId)
        {
            var instance = await _instanceService.GetByIdAsync(instanceId);
            if (instance == null)
                return NotFound($"Instance with ID {instanceId} not found.");

            return Ok(new
            {
                instance.Id,
                Workflow = instance.Workflow?.Name,
                CurrentStep = instance.CurrentStep?.Name,
                CurrentRole = instance.CurrentStep?.AssignedRole.ToString(),
                State = instance.State.ToString(),
                Transitions = instance.Transitions.Select(t => new
                {
                    t.Id,
                    t.Action,
                    t.FromStepId,
                    t.ToStepId,
                    t.FromState,
                    t.ToState,
                    t.Timestamp,
                    t.PerformedBy
                })
            });
        }

        // 🟤 4️⃣ Get all instances (Admin/Manager use)
        [HttpGet("all")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetAllInstances()
        {
            var instances = await _instanceService.GetAllAsync();

            var result = instances.Select(i => new
            {
                i.Id,
                Workflow = i.Workflow?.Name,
                CurrentStep = i.CurrentStep?.Name,
                AssignedRole = i.CurrentStep?.AssignedRole.ToString(),
                State = i.State.ToString()
            });

            return Ok(result);
        }
    }
}
