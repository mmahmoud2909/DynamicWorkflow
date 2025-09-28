using DynamicWorkflow.Core.Entities.Users;
using DynamicWorkflow.Core.Enums;

namespace DynamicWorkflow.Core.DTOs.StepDto
{
    public class WorkflowStepDto
    {
        private Status stepStatus1;
        private ActionType stepActionTypes1;
        private Roles assignedRole;

        public WorkflowStepDto(int id, string stepName, string? comments, Status stepStatus1, ActionType stepActionTypes1, bool isEndStep, Roles assignedRole, int workflowId)
        {
            Id = id;
            this.stepName = stepName;
            this.comments = comments;
            this.stepStatus1 = stepStatus1;
            this.stepActionTypes1 = stepActionTypes1;
            this.isEndStep = isEndStep;
            this.assignedRole = assignedRole;
            WorkflowId = workflowId;
        }

        public int Id { get; set; }
        public string stepName { get; set; }
        public string? comments { get; set; }
        public Status stepStatus { get; set; }
        public ActionType stepActionTypes { get; set; }
        public bool isEndStep { get; set; }
        public Roles AssignedRole { get; set; }
        public int WorkflowId { get; set; }
    }
}