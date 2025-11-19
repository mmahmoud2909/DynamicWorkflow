namespace DynamicWorkflow.Core.DTOs.StepDto
{
    public class StepDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Comments { get; set; }
        public int Order { get; set; }
        public bool IsEndStep { get; set; }

        public int WorkflowStatusId { get; set; }
        public WorkflowStatusDto WorkflowStatus { get; set; }

        public int ActionTypeEntityId { get; set; }
        public ActionTypeDto ActionTypeEntity { get; set; }

        public int AppRoleId { get; set; }
        public AppRoleDto AppRole { get; set; }
    }

    public class WorkflowStatusDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }


    public class ActionTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class AppRoleDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }


}
