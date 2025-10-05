using DynamicWorkflow.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DynamicWorkflow.Core.Entities
{

    public class WorkflowInstanceAction : BaseEntity
    {

        public int WorkflowInstanceId { get; set; }
        [JsonIgnore]
        public WorkflowInstance WorkflowInstance { get; set; }
        public Guid PerformedByUserId { get; set; }
        public ActionType ActionType { get; set; }
        public string? Comments { get; set; }
        public DateTime PerformedAt { get; set; }
        public int WorkFlowInstanceStepId { get; set; }
        public WorkFlowInstanceStep WorkFlowInstanceStep { get; set; }
        public  int WorkflowStepId {  get; set; }


    }
}
