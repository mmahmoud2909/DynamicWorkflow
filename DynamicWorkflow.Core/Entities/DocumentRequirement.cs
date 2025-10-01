
namespace DynamicWorkflow.Core.Entities
{
    public class DocumentRequirement:BaseEntity
    {
        public string DocumentName { get; set; }
        public int StepId { get; set; }
        public WorkflowStep Step { get; set; }
    }
}