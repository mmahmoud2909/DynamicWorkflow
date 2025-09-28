using DynamicWorkflow.Core.DTOs.StepDto;

namespace DynamicWorkflow.Core.DTOs.Workflow
{
    public class WorkflowDto
    {
        private int id;
        private List<WorkflowStepDto> workflowStepDtos;

        public WorkflowDto(int id, string name, string description, List<WorkflowStepDto> workflowStepDtos)
        {
            this.id = id;
            Name = name;
            Description = description;
            this.workflowStepDtos = workflowStepDtos;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        List<WorkflowStepDto> Steps { get; set; } = new List<WorkflowStepDto>();
    }
}
