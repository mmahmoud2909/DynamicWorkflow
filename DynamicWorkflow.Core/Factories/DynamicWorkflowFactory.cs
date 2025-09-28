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
                name = "Vacation Request Workflow",
                description = "Vaccation Approval Cycle",
                steps = new List<WorkflowStep>
                    {
                        new WorkflowStep
                        {
                            Id = 1,
                            stepName = "Employee Request for Vaccation",
                            stepActionTypes = ActionType.Create,
                            stepStatus = Status.InProgress,
                            AssignedRole = Roles.Employee,
                            isEndStep = false,
                        },

                        new WorkflowStep
                        {
                            Id = 2,
                            stepName = "Manager Approval",
                            stepActionTypes = ActionType.Accept,
                            stepStatus = Status.Pending,
                            AssignedRole = Roles.Manager,
                            isEndStep = false,
                        },
                        new WorkflowStep
                        {
                            Id = 3,
                            stepName = "HR Approval",
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
                name = "Manager Request Vaccation",
                description = "Manager Approval Cycle",
                steps = new List<WorkflowStep>
                    {
                        new WorkflowStep
                        {
                            Id = 1,
                            stepName="Manager Request For Vaccation",
                            stepActionTypes= ActionType.Create,
                            stepStatus = Status.Pending,
                            AssignedRole = Roles.Manager,
                            isEndStep = false,
                        },
                        new WorkflowStep
                        {
                            Id = 1,
                            stepName="HR Approval",
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
                name = "HR Request Vaccation",
                description = "HR Cycle",
                steps = new List<WorkflowStep>
                       {
                           new WorkflowStep
                           {
                               Id = 1,
                               stepName="HR Approval",
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
