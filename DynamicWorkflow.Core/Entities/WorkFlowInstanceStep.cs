using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWorkflow.Core.Entities
{
    public class WorkFlowInstanceStep:BaseEntity
    {
        public int InstanceId { get; set; }
        public WorkflowInstance ?Instance { get; set; }

        public int StepId { get; set; }
        public WorkflowStep Step { get; set; }
        public WorkflowInstanceAction WorkflowInstanceAction { get; set; }


        public string? PerformedByUserId { get; set; }  
        public string Status { get; set; } = "Pending"; // Pending, Completed, Rejected
        public string? Comments { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
