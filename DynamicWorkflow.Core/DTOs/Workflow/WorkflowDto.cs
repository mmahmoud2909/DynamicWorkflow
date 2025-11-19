using DynamicWorkflow.Core.DTOs.StepDto;

namespace DynamicWorkflow.Core.DTOs.Workflow
{
    public class WorkflowDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? WorkflowStatusId { get; set; }
        public string? WorkflowStatus { get; set; }
        public int? Order { get; set; }
        public List<WorkflowStepDto> Steps { get; set; }

        public WorkflowDto(int id, string name, string description, int? workflowStatusId, string? workflowStatus, int? order, List<WorkflowStepDto> steps)
        {
            Id = id;
            Name = name;
            Description = description;
            WorkflowStatusId = workflowStatusId;
            WorkflowStatus = workflowStatus;
            Order = order;
            Steps = steps;
        }
    }
}
