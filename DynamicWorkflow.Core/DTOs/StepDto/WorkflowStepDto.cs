namespace DynamicWorkflow.Core.DTOs.StepDto
{
    public class WorkflowStepDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Comments { get; set; }
        public int WorkflowStatusId { get; set; }
        public string? WorkflowStatus { get; set; }
        public int ActionTypeEntityId { get; set; }
        public string? ActionType { get; set; }
        public bool IsEndStep { get; set; }
        public int AppRoleId { get; set; }
        public string? AppRole { get; set; }
        public int WorkflowId { get; set; }

        public WorkflowStepDto(int id, string name, string? comments, int workflowStatusId, string? workflowStatus,
            int actionTypeEntityId, string? actionType, bool isEndStep, int appRoleId, string? appRole, int workflowId)
        {
            Id = id;
            Name = name;
            Comments = comments;
            WorkflowStatusId = workflowStatusId;
            WorkflowStatus = workflowStatus;
            ActionTypeEntityId = actionTypeEntityId;
            ActionType = actionType;
            IsEndStep = isEndStep;
            AppRoleId = appRoleId;
            AppRole = appRole;
            WorkflowId = workflowId;
        }
    }
}