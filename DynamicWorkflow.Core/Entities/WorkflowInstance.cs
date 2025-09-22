using DynamicWorkflow.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWorkflow.Core.Entities
{
    public class WorkflowInstance:BaseEntity
    {
        public int WorkflowId { get; set; }
        public Workflow? Workflow { get; set; }

        public int CurrentStepId { get; set; }
        public Status InstanceStatus {  get; set; }
        public WorkflowStep ?CurrentStep { get; set; }

    }
}
