using DynamicWorkflow.Core.DTOs.StepDto;

namespace DynamicWorkflow.Core.DTOs.Workflow
{
    public class WorkflowDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        List<WorkflowStepDto> Steps { get; set; } = new List<WorkflowStepDto>();

        public WorkflowDto(int id, string name, string description, List<WorkflowStepDto> steps)
        {
            this.Id = id;
            Name = name;
            Description = description;
            this.Steps = steps;
        }
    }
}
