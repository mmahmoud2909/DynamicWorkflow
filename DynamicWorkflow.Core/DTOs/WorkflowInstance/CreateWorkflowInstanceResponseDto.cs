using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWorkflow.Core.DTOs.WorkflowInstance
{
    public class CreateWorkflowInstanceResponseDto
    {
        public int Id { get; set; }
        public int WorkflowId { get; set; }
        public string State { get; set; }
        public int CurrentStepId { get; set; }
        public string CurrentStepName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
