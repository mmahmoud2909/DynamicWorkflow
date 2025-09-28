using DynamicWorkflow.Core.DTOs.StepDto;

namespace DynamicWorkflow.Core.DTOs.Workflow
{
    public class CreateWorkflowDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<WorkflowStepDto> Steps { get; set; }
    }
}
