using DynamicWorkflow.Core.Entities;
using DynamicWorkflow.Infrastructure.Identity;
using DynamicWorkflow.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DynamicWorkflow.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WorkflowController : ControllerBase
    {
        private readonly ApplicationIdentityDbContext _context;
        private readonly IWorkflowService _workflowService;

        public WorkflowController(
            ApplicationIdentityDbContext context,
            IWorkflowService workflowService)
        {
            _context = context;
            _workflowService = workflowService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkflowDto>>> GetAllWorkflows()
        {
            try
            {
                var workflows = await _workflowService.GetAllWorkflowsAsync();
                var workflowDtos = workflows.Select(w => new WorkflowDto
                {
                    Id = w.Id,
                    Name = w.Name,
                    Description = w.Description,
                    ParentWorkflowId = w.ParentWorkflowId,
                    Order = w.Order,
                    WorkflowStatusId = w.WorkflowStatusId,
                    WorkflowStatus = w.WorkflowStatus?.Name,
                    StepCount = w.Steps.Count,
                    InstanceCount = w.Instances.Count,
                    CreatedAt = w.CreatedAt,
                    CreatedBy = w.CreatedBy,
                    UpdatedAt = w.UpdatedAt,
                    UpdatedBy = w.UpdatedBy
                }).ToList();

                return Ok(workflowDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving workflows", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Workflow>> GetWorkflow(int id)
        {
            try
            {
                var workflow = await _workflowService.GetWorkflowByIdAsync(id);

                if (workflow == null)
                    return NotFound(new { message = $"Workflow with ID {id} not found" });

                return Ok(workflow);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving workflow", error = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<Workflow>> CreateWorkflow([FromBody] CreateWorkflowRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();

                var workflow = new Workflow
                {
                    Name = request.Name,
                    Description = request.Description,
                    ParentWorkflowId = request.ParentWorkflowId,
                    Order = request.Order,
                    WorkflowStatusId = request.WorkflowStatusId,
                    CreatedBy = userId,
                    CreatedAt = DateTime.UtcNow
                };

                var savedWorkflow = await _workflowService.SaveWorkflowAsync(workflow, userId);

                return CreatedAtAction(
                    nameof(GetWorkflow),
                    new { id = savedWorkflow.Id },
                    savedWorkflow);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating workflow", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<Workflow>> UpdateWorkflow(int id, [FromBody] UpdateWorkflowRequest request)
        {
            try
            {
                var workflow = await _context.Workflows.FindAsync(id);

                if (workflow == null)
                    return NotFound(new { message = $"Workflow with ID {id} not found" });

                var userId = GetCurrentUserId();

                workflow.Name = request.Name;
                workflow.Description = request.Description;
                workflow.Order = request.Order;
                workflow.WorkflowStatusId = request.WorkflowStatusId;
                workflow.UpdatedBy = userId;
                workflow.UpdatedAt = DateTime.UtcNow;

                await _workflowService.SaveWorkflowAsync(workflow, userId);

                return Ok(workflow);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating workflow", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult> DeleteWorkflow(int id)
        {
            try
            {
                var result = await _workflowService.DeleteWorkflowAsync(id);

                if (!result)
                    return NotFound(new { message = $"Workflow with ID {id} not found" });

                return Ok(new { message = "Workflow deleted successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting workflow", error = ex.Message });
            }
        }

        //[HttpPost("{id}/start")]
        //public async Task<ActionResult<WorkflowInstance>> StartWorkflow(int id)
        //{
        //    try
        //    {
        //        var userId = GetCurrentUserId();
        //        var user = await _context.ApplicationUsers.FindAsync(Guid.Parse(userId));

        //        if (user == null)
        //            return Unauthorized(new { message = "User not found" });

        //        var result = await _workflowService.StartWorkflowAsync(id, user);

        //        if (!result)
        //            return BadRequest(new { message = "Failed to start workflow" });

        //        // Get the created instance
        //        var workflow = await _workflowService.GetWorkflowByIdAsync(id);
        //        var instance = workflow?.Instances.OrderByDescending(i => i.CreatedAt).FirstOrDefault();

        //        return CreatedAtAction(
        //            nameof(GetInstance),
        //            new { id = instance?.Id },
        //            instance);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { message = "Error starting workflow", error = ex.Message });
        //    }
        //}

        [HttpPost("{id}/instance")]
        public async Task<ActionResult<WorkflowInstance>> CreateInstance(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var user = await _context.ApplicationUsers.FindAsync(Guid.Parse(userId));

                if (user == null)
                    return Unauthorized(new { message = "User not found" });

                var instance = await _workflowService.CreateInstanceAsync(id, user);

                return CreatedAtAction(
                    nameof(GetInstance),
                    new { id = instance.Id },
                    instance);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating workflow instance", error = ex.Message });
            }
        }

        [HttpGet("instance/{id}")]
        public async Task<ActionResult<WorkflowInstance>> GetInstance(int id)
        {
            try
            {
                var instance = await _workflowService.GetInstanceByIdAsync(id);

                if (instance == null)
                    return NotFound(new { message = $"Workflow instance with ID {id} not found" });

                return Ok(instance);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving workflow instance", error = ex.Message });
            }
        }

        //[HttpPost("instance/{instanceId}/cancel")]
        //public async Task<ActionResult> CancelWorkflow(int instanceId, [FromBody] CancelWorkflowRequest request)
        //{
        //    try
        //    {
        //        var userId = GetCurrentUserId();

        //        var result = await _workflowService.CancelWorkflowAsync(instanceId, userId, request.Reason);

        //        if (!result)
        //            return BadRequest(new { message = "Failed to cancel workflow" });

        //        return Ok(new { message = "Workflow cancelled successfully" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { message = "Error cancelling workflow", error = ex.Message });
        //    }
        //}

        //[HttpPost("instance/{instanceId}/restart")]
        //public async Task<ActionResult> RestartWorkflow(int instanceId)
        //{
        //    try
        //    {
        //        var userId = GetCurrentUserId();

        //        var result = await _workflowService.RestartWorkflowAsync(instanceId, userId);

        //        if (!result)
        //            return BadRequest(new { message = "Failed to restart workflow" });

        //        return Ok(new { message = "Workflow restarted successfully" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { message = "Error restarting workflow", error = ex.Message });
        //    }
        //}

        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("User ID not found in token");
        }
    }

    public class WorkflowDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? ParentWorkflowId { get; set; }
        public int? Order { get; set; }
        public int? WorkflowStatusId { get; set; }
        public string WorkflowStatus { get; set; }
        public int StepCount { get; set; }
        public int InstanceCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class CreateWorkflowRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int? ParentWorkflowId { get; set; }
        public int Order { get; set; }
        public int WorkflowStatusId { get; set; }
    }

    public class UpdateWorkflowRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
        public int WorkflowStatusId { get; set; }
    }

    public class CancelWorkflowRequest
    {
        public string? Reason { get; set; }
    }
}