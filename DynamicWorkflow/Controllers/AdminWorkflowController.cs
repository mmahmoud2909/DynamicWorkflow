using DynamicWorkflow.Core.DTOs.StepDto;
using DynamicWorkflow.Core.DTOs.Transition;
using DynamicWorkflow.Core.DTOs.Workflow;
using DynamicWorkflow.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DynamicWorkflow.APIs.Controllers
{
    [ApiController]
    [Route("api/admin/workflows")]
    [Authorize(Policy = "AdminOnly")]
    public class AdminWorkflowController : ControllerBase
    {
        private readonly IAdminWorkflowService _svc;
        public AdminWorkflowController(IAdminWorkflowService svc) => _svc = svc;

        [HttpGet("GetAllWorkflows")]
        public async Task<IActionResult> GetAll() => Ok(await _svc.GetAllWorkflowsAsync());

        [HttpGet("GetWorkflowById/{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var wf = await _svc.GetWorkflowByIdAsync(id);
            return wf == null ? NotFound() : Ok(wf);
        }

        [HttpPost("CreateWorkflow")]
        public async Task<IActionResult> Create([FromBody] CreateWorkflowDto dto)
        {
            var id = await _svc.CreateWorkflowAsync(dto);
            return CreatedAtAction(nameof(Get), new { id }, id);
        }

        [HttpPut("UpdateWorkflow/{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateWorkflowDto dto)
        {
            await _svc.UpdateWorkflowAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("DeleteWorkflow/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _svc.DeleteWorkflowAsync(id);
            return NoContent();
        }

        // Steps
        [HttpPost("AddStep/{workflowId:int}")]
        public async Task<IActionResult> AddStep(int workflowId, [FromBody] CreateStepDto dto)
        {
            var stepId = await _svc.AddStepAsync(workflowId, dto);
            return CreatedAtAction(nameof(Get), new { id = workflowId }, new { stepId });
        }

        [HttpPut("UpdateStep/{stepId:int}")]
        public async Task<IActionResult> UpdateStep(int stepId, [FromBody] UpdateStepDto dto)
        {
            await _svc.UpdateStepAsync(stepId, dto);
            return NoContent();
        }

        [HttpDelete("DeleteStep/{stepId:int}")]
        public async Task<IActionResult> DeleteStep(int stepId)
        {
            await _svc.DeleteStepAsync(stepId);
            return NoContent();
        }

        // Transitions
        [HttpPost("AddTransition/{workflowId:int}")]
        public async Task<IActionResult> AddTransition(int workflowId, [FromBody] CreateTransitionDto dto)
        {
            var id = await _svc.AddTransitionAsync(workflowId, dto);
            return CreatedAtAction(nameof(Get), new { id = workflowId }, new { transitionId = id });
        }

        [HttpDelete("DeleteTransition/{id:int}")]
        public async Task<IActionResult> DeleteTransition(int id)
        {
            await _svc.DeleteTransitionAsync(id);
            return NoContent();
        }
    }

}
