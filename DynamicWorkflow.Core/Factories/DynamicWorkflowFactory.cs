using DynamicWorkflow.Core.Entities;
using DynamicWorkflow.Core.Enums;

namespace DynamicWorkflow.Core.Factories
{
    public static class DynamicWorkflowFactory
    {
        public static Workflow CreateWorkflow(Roles RequesterRole) => RequesterRole
            switch
        {
            Roles.Employee => new Workflow
            {
                Id = 1,
                Name = "Vacation Request Workflow",
                Description = "Vaccation Approval Cycle",
                Steps = new List<WorkflowStep>
                    {
                        new WorkflowStep
                        {
                            Id = 1,
                            Name = "Employee Request for Vaccation",
                            stepActionTypes = ActionType.Create,
                            stepStatus = Status.InProgress,
                            AssignedRole = Roles.Employee,
                            isEndStep = false,
                        },

                        new WorkflowStep
                        {
                            Id = 2,
                            Name = "Manager Approval",
                            stepActionTypes = ActionType.Accept,
                            stepStatus = Status.Pending,
                            AssignedRole = Roles.Manager,
                            isEndStep = false,
                        },
                        new WorkflowStep
                        {
                            Id = 3,
                            Name = "HR Approval",
                            stepActionTypes = ActionType.Accept,
                            stepStatus = Status.Accepted,
                            AssignedRole = Roles.HR,
                            isEndStep = true,
                        }
                    }
            },
            Roles.Manager => new Workflow
            {
                Id = 2,
                Name = "Manager Request Vaccation",
                Description = "Manager Approval Cycle",
                Steps = new List<WorkflowStep>
                    {
                        new WorkflowStep
                        {
                            Id = 1,
                            Name="Manager Request For Vaccation",
                            stepActionTypes= ActionType.Create,
                            stepStatus = Status.Pending,
                            AssignedRole = Roles.Manager,
                            isEndStep = false,
                        },
                        new WorkflowStep
                        {
                            Id = 1,
                            Name="HR Approval",
                            stepActionTypes= ActionType.Accept,
                            stepStatus = Status.Accepted,
                            AssignedRole = Roles.HR,
                            isEndStep = true,
                        }
                    }
            },
            Roles.HR => new Workflow
            {
                Id = 3,
                Name = "HR Request Vaccation",
                Description = "HR Cycle",
                Steps = new List<WorkflowStep>
                       {
                           new WorkflowStep
                           {
                               Id = 1,
                               Name="HR Approval",
                               stepActionTypes= ActionType.Accept,
                               stepStatus = Status.InProgress,
                               AssignedRole = Roles.HR,
                               isEndStep = true,
                           }
                       }
            },
           
           _ =>throw new Exception("Not Supported Role ")
        };
    }
}
