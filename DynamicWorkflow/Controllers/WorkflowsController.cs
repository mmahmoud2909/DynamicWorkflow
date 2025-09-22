using DynamicWorkflow.Core.Enums;
using DynamicWorkflow.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DynamicWorkflow.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkflowsController : ControllerBase
    {
        private readonly WorkflowService _service;
        public WorkflowsController(WorkflowService service) => _service = service;

        [HttpPost("{id}/trigger/{trigger}")]
        public async Task<IActionResult> Trigger(Guid id, ActionType action)
        {
            await _service.TriggerAsync(id, action);
            return Ok();
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromQuery] string user)
        {
            var wf = await _service.CreateWorkflow(user);
            return Ok(wf);
        }

        [HttpPost("{id}/fire")]
        public async Task<IActionResult> Fire(Guid id, [FromQuery] ActionType action, [FromQuery] string user)
        {
            var wf = await _service.(id, action, user);
            return Ok(wf);
        }
    }

}
