using DynamicWorkflow.Core.Enums;
using DynamicWorkflow.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace DynamicWorkflow.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkflowController : ControllerBase
    {
        private readonly IWorkflowService _service;

        public WorkflowController(IWorkflowService service)
        {
            _service = service;
        }

        [HttpPost("{id}/trigger/{trigger}")]
        public async Task<IActionResult> Trigger(int id, ActionType action)
        {
            await _service.TriggerAsync(id, action, "mariam");
            return Ok();
        }

        //[HttpPost("{id}/fire")]
        //public async Task<IActionResult> Fire( [FromQuery] ActionType action, [FromQuery] string user)
        //{
        //    var wf = await _service.TriggerAsync(1, action, user);
        //    return Ok(wf);
        //}
    }

}
