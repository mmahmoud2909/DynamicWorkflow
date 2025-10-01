
namespace DynamicWorkflow.Core.Entities
{
    public class StepRole
    {
        public int Id { get; set; }
        public int StepId { get; set; }
        public string RoleName { get; set; }    // e.g. "Manager"
        public string ActorName { get; set; }   // e.g. "Hany Gawish"
        public bool IsMandatory { get; set; }

        public WorkflowStep Step { get; set; }
    }
}
