using DynamicWorkflow.Core.Enums;
using DynamicWorkflow.Core.Factories;
using DynamicWorkflow.Infrastructure.Data;
using DynamicWorkflow.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace DynamicWorkflow.APIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StepController : ControllerBase
    {
        private readonly StepService _stepService;
        public StepController(StepService stepService)
        {
            _stepService = stepService;
        }

        //[HttpGet]
        //public object GetAllSteps()
        //{
        //    var result = _stepService.GetAllSteps(Workflow );
        //}

        [HttpGet("{id}")]
        public IActionResult GetWorkflowById(int id)
        {
            var workflow = WorkflowRepository.GetWorkflow();
            return Ok(workflow);
        }

        [HttpPost("{workflowId}/step/{stepId}/action")]
        public IActionResult MakeAction(int workflowId, int stepId, [FromQuery] ActionType action)
        {
            var workflow = WorkflowRepository.GetWorkflow();
            _stepService.MakeAction(workflow, stepId, action);
            return Ok(workflow);
        }

        [HttpGet("start/{role}")]
       
        public IActionResult StartWorkflowAccordingToRole(Roles role)
        {
            var workflow =DynamicWorkflowFactory.CreateWorkflow(role);
            return Ok(workflow);

        }
    }
}
