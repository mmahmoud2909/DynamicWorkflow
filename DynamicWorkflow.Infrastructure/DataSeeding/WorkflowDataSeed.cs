
using DynamicWorkflow.Core.Entities;
using DynamicWorkflow.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace DynamicWorkflow.Infrastructure.DataSeeding
{
    //public class WorkflowDataSeed
    //{
    //    public static void SeedWorkflows(this ModelBuilder modelBuilder)
    //    {
    //        // ================= MAIN HIERARCHY =================
    //        modelBuilder.Entity<Workflow>().HasData(
    //            new Workflow { Id = 1, Name = "Raw Material Workflow", ParentWorkflowId = null },
    //            new Workflow { Id = 2, Name = "PR Approval", ParentWorkflowId = 1 },
    //            new Workflow { Id = 3, Name = "PO Approval", ParentWorkflowId = 1 },
    //            new Workflow { Id = 4, Name = "GRN Acceptance", ParentWorkflowId = 1 },
    //            new Workflow { Id = 5, Name = "Warehouse", ParentWorkflowId = 1 },
    //            new Workflow { Id = 6, Name = "Invoice Payable", ParentWorkflowId = 3 } // Invoice after PO
    //        );

    //        // ================= PR APPROVAL (4 STEPS) =================
    //        modelBuilder.Entity<WorkflowStep>().HasData(
    //            new WorkflowStep { Id = 1, WorkflowId = 2, Order = 1, Role = "User", PersonName = "Mariam Shoukry", IsMandatory = true },
    //            new WorkflowStep { Id = 2, WorkflowId = 2, Order = 2, Role = "Manager", PersonName = "Hany Gawish", IsMandatory = true },
    //            new WorkflowStep { Id = 3, WorkflowId = 2, Order = 3, Role = "Director", PersonName = "Mohamed Hawary", IsMandatory = true },
    //            new WorkflowStep { Id = 4, WorkflowId = 2, Order = 4, Role = "C-Level", PersonName = "Mohamed Aboud", IsMandatory = true }
    //        );

    //        // ================= PO APPROVAL (7 STEPS) =================
    //        modelBuilder.Entity<WorkflowStep>().HasData(
    //            new WorkflowStep { Id = 5, WorkflowId = 3, Order = 1, Role = "User", PersonName = "Mah.Kenawy / Moh Soliman", IsMandatory = true },
    //            new WorkflowStep { Id = 6, WorkflowId = 3, Order = 2, Role = "Manager", PersonName = "Ahmed Youssef", IsMandatory = true },
    //            new WorkflowStep { Id = 7, WorkflowId = 3, Order = 3, Role = "Technical", PersonName = "Asraf Sakr", IsMandatory = true },
    //            new WorkflowStep { Id = 8, WorkflowId = 3, Order = 4, Role = "Planning", PersonName = "Hany Gawish", IsMandatory = true },
    //            new WorkflowStep { Id = 9, WorkflowId = 3, Order = 5, Role = "Treasury", PersonName = "Haitham Gabr", IsMandatory = true },
    //            new WorkflowStep { Id = 10, WorkflowId = 3, Order = 6, Role = "Director", PersonName = "Manal Adam", IsMandatory = true },
    //            new WorkflowStep { Id = 11, WorkflowId = 3, Order = 7, Role = "C-Level", PersonName = "Ahmed Mohsen", IsMandatory = true }
    //        );

    //        // ================= GRN ACCEPTANCE (8 STEPS) =================
    //        modelBuilder.Entity<WorkflowStep>().HasData(
    //            new WorkflowStep { Id = 12, WorkflowId = 4, Order = 1, Role = "Procurement", PersonName = "Proc Section Head", IsMandatory = true },
    //            new WorkflowStep { Id = 13, WorkflowId = 4, Order = 2, Role = "Warehouse Keeper", PersonName = "LV Store Keeper - Saudi Mohamed", IsMandatory = true },
    //            new WorkflowStep { Id = 14, WorkflowId = 4, Order = 3, Role = "QC", PersonName = "Physical Lab Section Head - Mohamed Ghoher", IsMandatory = true },
    //            new WorkflowStep { Id = 15, WorkflowId = 4, Order = 4, Role = "QC Senior Manager", PersonName = "Hany Eid", IsMandatory = true },
    //            new WorkflowStep { Id = 16, WorkflowId = 4, Order = 5, Role = "Technical Director", PersonName = "Ashraf Saqr", IsMandatory = true },
    //            new WorkflowStep { Id = 17, WorkflowId = 4, Order = 6, Role = "Warehouse Supervisor", PersonName = "Medhat Salah", IsMandatory = true },
    //            new WorkflowStep { Id = 18, WorkflowId = 4, Order = 7, Role = "Warehouse Manager", PersonName = "Walid Hafez", IsMandatory = true },
    //            new WorkflowStep { Id = 19, WorkflowId = 4, Order = 8, Role = "Plant Director", PersonName = "Mohamed Hawary", IsMandatory = true }
    //        );

    //        // ================= WAREHOUSE (6 STEPS) =================
    //        modelBuilder.Entity<WorkflowStep>().HasData(
    //            new WorkflowStep { Id = 20, WorkflowId = 5, Order = 1, Role = "LV Store Keeper (Metal)", PersonName = "Ibrahim Hassan", IsMandatory = true },
    //            new WorkflowStep { Id = 21, WorkflowId = 5, Order = 2, Role = "Warehouse Supervisor", PersonName = "Medhat Salah", IsMandatory = true },
    //            new WorkflowStep { Id = 22, WorkflowId = 5, Order = 3, Role = "Warehouse Manager", PersonName = "Walid Hafez", IsMandatory = true },
    //            new WorkflowStep { Id = 23, WorkflowId = 5, Order = 4, Role = "Procurement Section Head (Metal)", PersonName = "Mah.Kenawy", IsMandatory = true },
    //            new WorkflowStep { Id = 24, WorkflowId = 5, Order = 5, Role = "Procurement Manager", PersonName = "Ahmed Youssef", IsMandatory = true },
    //            new WorkflowStep { Id = 25, WorkflowId = 5, Order = 6, Role = "Supply Chain Director", PersonName = "Manal Adam", IsMandatory = true }
    //        );

    //        // ================= INVOICE PAYABLE (3 STEPS) =================
    //        modelBuilder.Entity<WorkflowStep>().HasData(
    //            new WorkflowStep { Id = 26, WorkflowId = 6, Order = 1, Role = "User", PersonName = "Walid Hafez", IsMandatory = true },
    //            new WorkflowStep { Id = 27, WorkflowId = 6, Order = 2, Role = "Manager", PersonName = "Mohamed Hawary", IsMandatory = true },
    //            new WorkflowStep { Id = 28, WorkflowId = 6, Order = 3, Role = "Director", PersonName = "Moh Soliman", IsMandatory = true }
    //        );
    //    }
    //}

    public static class WorkflowSeedData
    {
        public static List<Workflow> GetWorkflows()
        {
            var workflows = new List<Workflow>();
            var workflowId = 1;
            var stepId = 1;
            var transitionId = 1;

            // Main Parent Workflow - LV Plant
            var lvPlantWorkflow = new Workflow
            {
                Id = workflowId++,
                Name = "LV Plant Workflow",
                Description = "Complete Low Voltage Plant Approval Process",
                ParentWorkflowId = null,
                Order = 1
            };
            workflows.Add(lvPlantWorkflow);

            // 1. PR Approval Workflow (Child of LV Plant)
            var prApprovalWorkflow = CreatePRApprovalWorkflow(workflowId++, lvPlantWorkflow.Id, 1, ref stepId, ref transitionId);
            workflows.Add(prApprovalWorkflow);

            // 2. PO Approval Workflow (Child of LV Plant)
            var poApprovalWorkflow = CreatePOApprovalWorkflow(workflowId++, lvPlantWorkflow.Id, 2, ref stepId, ref transitionId);
            workflows.Add(poApprovalWorkflow);

            // 3. GRN Acceptance Workflow (Child of LV Plant)
            var grnAcceptanceWorkflow = CreateGRNAcceptanceWorkflow(workflowId++, lvPlantWorkflow.Id, 3, ref stepId, ref transitionId);
            workflows.Add(grnAcceptanceWorkflow);

            // 4. Warehouse Workflow (Child of LV Plant)
            var warehouseWorkflow = CreateWarehouseWorkflow(workflowId++, lvPlantWorkflow.Id, 4, ref stepId, ref transitionId);
            workflows.Add(warehouseWorkflow);

            // 5. Final Approval Workflow (Child of LV Plant)
            var finalApprovalWorkflow = CreateFinalApprovalWorkflow(workflowId++, lvPlantWorkflow.Id, 5, ref stepId, ref transitionId);
            workflows.Add(finalApprovalWorkflow);

            // 6. Invoice Payable Workflow (Child of LV Plant)
            var invoicePayableWorkflow = CreateInvoicePayableWorkflow(workflowId++, lvPlantWorkflow.Id, 6, ref stepId, ref transitionId);
            workflows.Add(invoicePayableWorkflow);

            return workflows;
        }

        private static Workflow CreatePRApprovalWorkflow(int id, int parentId, int order, ref int stepId, ref int transitionId)
        {
            var workflow = new Workflow
            {
                Id = id,
                Name = "PR Approval Workflow",
                Description = "Purchase Request Approval Process",
                ParentWorkflowId = parentId,
                Order = order,
                Steps = new List<WorkflowStep>(),
                Transitions = new List<WorkflowTransition>()
            };

            // Step 1: System Approval
            var systemApproval = new WorkflowStep
            {
                Id = stepId++,
                Name = "PR System Approval",
                Order = 1,
                stepStatus = Status.Pending,
                stepActionTypes = ActionType.SystemApproval,
                isEndStep = false,
                AssignedRole = Roles.User,
                WorkflowId = id,
                Roles = new List<StepRole>
                {
                    new StepRole { Id = 1, StepId = stepId - 1, RoleName = "System", ActorName = "Automated System", IsMandatory = true }
                }
            };

            // Step 2: User Approval
            var userApproval = new WorkflowStep
            {
                Id = stepId++,
                Name = "PR User Approval",
                Order = 2,
                stepStatus = Status.Pending,
                stepActionTypes = ActionType.UserApproval,
                isEndStep = false,
                AssignedRole = Roles.User,
                WorkflowId = id,
                Roles = new List<StepRole>
                {
                    new StepRole { Id = 2, StepId = stepId - 1, RoleName = "RM Planning Team Leader", ActorName = "Mariam Shoukry", IsMandatory = true }
                }
            };

            // Step 3: Manager Approval
            var managerApproval = new WorkflowStep
            {
                Id = stepId++,
                Name = "PR Manager Approval",
                Order = 3,
                stepStatus = Status.Pending,
                stepActionTypes = ActionType.ManagerApproval,
                isEndStep = false,
                AssignedRole = Roles.Manager,
                WorkflowId = id,
                Roles = new List<StepRole>
                {
                    new StepRole { Id = 3, StepId = stepId - 1, RoleName = "Planning Manager", ActorName = "Hany Gawish", IsMandatory = true }
                }
            };

            // Step 4: Director Approval
            var directorApproval = new WorkflowStep
            {
                Id = stepId++,
                Name = "PR Director Approval",
                Order = 4,
                stepStatus = Status.Pending,
                stepActionTypes = ActionType.DirectorApproval,
                isEndStep = false,
                AssignedRole = Roles.Director,
                WorkflowId = id,
                Roles = new List<StepRole>
                {
                    new StepRole { Id = 4, StepId = stepId - 1, RoleName = "Plant Director", ActorName = "Mohamed Hawary", IsMandatory = true }
                }
            };

            // Step 5: C-Level Approval
            var clevelApproval = new WorkflowStep
            {
                Id = stepId++,
                Name = "PR C-Level Approval",
                Order = 5,
                stepStatus = Status.Pending,
                stepActionTypes = ActionType.CLevelApproval,
                isEndStep = true,
                AssignedRole = Roles.CLevel,
                WorkflowId = id,
                Roles = new List<StepRole>
                {
                    new StepRole { Id = 5, StepId = stepId - 1, RoleName = "COO", ActorName = "Mohamed Aboud", IsMandatory = true }
                }
            };

            workflow.Steps.Add(systemApproval);
            workflow.Steps.Add(userApproval);
            workflow.Steps.Add(managerApproval);
            workflow.Steps.Add(directorApproval);
            workflow.Steps.Add(clevelApproval);

            // Create transitions
            workflow.Transitions.Add(CreateTransition(transitionId++, id, systemApproval.Id, userApproval.Id, ActionType.SystemApproval));
            workflow.Transitions.Add(CreateTransition(transitionId++, id, userApproval.Id, managerApproval.Id, ActionType.UserApproval));
            workflow.Transitions.Add(CreateTransition(transitionId++, id, managerApproval.Id, directorApproval.Id, ActionType.ManagerApproval));
            workflow.Transitions.Add(CreateTransition(transitionId++, id, directorApproval.Id, clevelApproval.Id, ActionType.DirectorApproval));

            return workflow;
        }

        private static Workflow CreatePOApprovalWorkflow(int id, int parentId, int order, ref int stepId, ref int transitionId)
        {
            var workflow = new Workflow
            {
                Id = id,
                Name = "PO Approval Workflow",
                Description = "Purchase Order Approval Process",
                ParentWorkflowId = parentId,
                Order = order,
                Steps = new List<WorkflowStep>(),
                Transitions = new List<WorkflowTransition>()
            };

            // Steps for PO Approval
            var steps = new[]
            {
                new { Name = "PO System Approval", Action = ActionType.SystemApproval, Role = Roles.User, RoleName = "System", Actor = "Automated System" },
                new { Name = "PO User Approval", Action = ActionType.UserApproval, Role = Roles.User, RoleName = "Proc Section Head", Actor = "Mah.Kenawy / Moh Soliman" },
                new { Name = "PO Manager Approval", Action = ActionType.ManagerApproval, Role = Roles.Manager, RoleName = "Procurement Manager", Actor = "Ahmed Youssef" },
                new { Name = "PO Technical Approval", Action = ActionType.TechnicalApproval, Role = Roles.Technical, RoleName = "Technical Director", Actor = "Ashraf Sakr" },
                new { Name = "PO Planning Approval", Action = ActionType.PlanningApproval, Role = Roles.Planning, RoleName = "Planning Manager", Actor = "Hany Gawish" },
                new { Name = "PO Treasury Approval", Action = ActionType.TreasuryApproval, Role = Roles.Treasury, RoleName = "Treasury Director", Actor = "Haitham Gabr" },
                new { Name = "PO Director Approval", Action = ActionType.DirectorApproval, Role = Roles.Director, RoleName = "S. Chain Director", Actor = "Manal Adam" },
                new { Name = "PO C-Level Approval", Action = ActionType.CLevelApproval, Role = Roles.CLevel, RoleName = "CEO", Actor = "Ahmed Mohsen" }
            };

            for (int i = 0; i < steps.Length; i++)
            {
                var step = new WorkflowStep
                {
                    Id = stepId++,
                    Name = steps[i].Name,
                    Order = i + 1,
                    stepStatus = Status.Pending,
                    stepActionTypes = steps[i].Action,
                    isEndStep = i == steps.Length - 1,
                    AssignedRole = steps[i].Role,
                    WorkflowId = id,
                    Roles = new List<StepRole>
                    {
                        new StepRole { Id = stepId, StepId = stepId - 1, RoleName = steps[i].RoleName, ActorName = steps[i].Actor, IsMandatory = true }
                    }
                };
                workflow.Steps.Add(step);
            }

            // Create transitions between steps
            for (int i = 0; i < workflow.Steps.Count - 1; i++)
            {
                workflow.Transitions.Add(CreateTransition(
                    transitionId++,
                    id,
                    workflow.Steps.ElementAt(i).Id,
                    workflow.Steps.ElementAt(i + 1).Id,
                    workflow.Steps.ElementAt(i).stepActionTypes
                ));
            }

            return workflow;
        }

        private static Workflow CreateGRNAcceptanceWorkflow(int id, int parentId, int order, ref int stepId, ref int transitionId)
        {
            var workflow = new Workflow
            {
                Id = id,
                Name = "GRN Acceptance Workflow",
                Description = "Goods Received Note Acceptance Process",
                ParentWorkflowId = parentId,
                Order = order,
                Steps = new List<WorkflowStep>(),
                Transitions = new List<WorkflowTransition>()
            };

            var steps = new[]
            {
                new { Name = "System Approval", Action = ActionType.SystemApproval, Role = Roles.User, RoleName = "System", Actor = "Automated System" },
                new { Name = "Procurement Approval", Action = ActionType.ProcurementApproval, Role = Roles.Procurement, RoleName = "Proc Section Head", Actor = "Mah.Kenawy / Moh Soliman" },
                new { Name = "Warehouse Approval", Action = ActionType.WarehouseApproval, Role = Roles.Warehouse, RoleName = "LV Store Keeper", Actor = "Saudi Mohamed" },
                new { Name = "QC Approval", Action = ActionType.QcApproval, Role = Roles.QC, RoleName = "Physical Lab Section Head", Actor = "Mohamed Ghoher" },
                new { Name = "QC Manager Approval", Action = ActionType.ManagerApproval, Role = Roles.Manager, RoleName = "QC Senior Manager", Actor = "Hany Eid" },
                new { Name = "Technical Director Approval", Action = ActionType.DirectorApproval, Role = Roles.Director, RoleName = "Technical Director", Actor = "Ashraf Saqr" }
            };

            for (int i = 0; i < steps.Length; i++)
            {
                var step = new WorkflowStep
                {
                    Id = stepId++,
                    Name = $"GRN {steps[i].Name}",
                    Order = i + 1,
                    stepStatus = Status.Pending,
                    stepActionTypes = steps[i].Action,
                    isEndStep = i == steps.Length - 1,
                    AssignedRole = steps[i].Role,
                    WorkflowId = id,
                    Roles = new List<StepRole>
                    {
                        new StepRole { Id = stepId, StepId = stepId - 1, RoleName = steps[i].RoleName, ActorName = steps[i].Actor, IsMandatory = true }
                    }
                };
                workflow.Steps.Add(step);
            }

            for (int i = 0; i < workflow.Steps.Count - 1; i++)
            {
                workflow.Transitions.Add(CreateTransition(
                    transitionId++,
                    id,
                    workflow.Steps.ElementAt(i).Id,
                    workflow.Steps.ElementAt(i + 1).Id,
                    workflow.Steps.ElementAt(i).stepActionTypes
                ));
            }

            return workflow;
        }

        private static Workflow CreateWarehouseWorkflow(int id, int parentId, int order, ref int stepId, ref int transitionId)
        {
            var workflow = new Workflow
            {
                Id = id,
                Name = "Warehouse Workflow",
                Description = "Warehouse Processing and GRN Generation",
                ParentWorkflowId = parentId,
                Order = order,
                Steps = new List<WorkflowStep>(),
                Transitions = new List<WorkflowTransition>()
            };

            // Non-metal path
            var stepsNonMetal = new[]
            {
                new { Name = "System Reporting", Action = ActionType.SystemApproval, Role = Roles.User, RoleName = "System", Actor = "Automated System" },
                new { Name = "GRN Generation Non-Metal", Action = ActionType.GrnGeneration, Role = Roles.StoreKeeper, RoleName = "LV Store Keeper Non metal", Actor = "Saudi Mohamed" },
                new { Name = "Supervisor Approval Non-Metal", Action = ActionType.ManagerApproval, Role = Roles.Supervisor, RoleName = "LV Store Supervisor", Actor = "Medhat Salah" },
                new { Name = "WH Manager Approval Non-Metal", Action = ActionType.ManagerApproval, Role = Roles.Manager, RoleName = "WH Manager", Actor = "Walid Hafez" }
            };

            for (int i = 0; i < stepsNonMetal.Length; i++)
            {
                var step = new WorkflowStep
                {
                    Id = stepId++,
                    Name = stepsNonMetal[i].Name,
                    Order = i + 1,
                    stepStatus = Status.Pending,
                    stepActionTypes = stepsNonMetal[i].Action,
                    isEndStep = i == stepsNonMetal.Length - 1,
                    AssignedRole = stepsNonMetal[i].Role,
                    WorkflowId = id,
                    Roles = new List<StepRole>
                    {
                        new StepRole { Id = stepId, StepId = stepId - 1, RoleName = stepsNonMetal[i].RoleName, ActorName = stepsNonMetal[i].Actor, IsMandatory = true }
                    }
                };
                workflow.Steps.Add(step);
            }

            for (int i = 0; i < stepsNonMetal.Length - 1; i++)
            {
                workflow.Transitions.Add(CreateTransition(
                    transitionId++,
                    id,
                    workflow.Steps.ElementAt(i).Id,
                    workflow.Steps.ElementAt(i + 1).Id,
                    workflow.Steps.ElementAt(i).stepActionTypes
                ));
            }

            return workflow;
        }

        private static Workflow CreateFinalApprovalWorkflow(int id, int parentId, int order, ref int stepId, ref int transitionId)
        {
            var workflow = new Workflow
            {
                Id = id,
                Name = "Final Approval Workflow",
                Description = "Final System Approval Process",
                ParentWorkflowId = parentId,
                Order = order,
                Steps = new List<WorkflowStep>(),
                Transitions = new List<WorkflowTransition>()
            };

            var step = new WorkflowStep
            {
                Id = stepId++,
                Name = "Final System Approval",
                Order = 1,
                stepStatus = Status.Pending,
                stepActionTypes = ActionType.SystemApproval,
                isEndStep = true,
                AssignedRole = Roles.Director,
                WorkflowId = id,
                Roles = new List<StepRole>
                {
                    new StepRole { Id = stepId, StepId = stepId - 1, RoleName = "Director",  ActorName = "Mohamed Hawary", IsMandatory = true }
                }
            };

            workflow.Steps.Add(step);
            return workflow;
        }

        private static Workflow CreateInvoicePayableWorkflow(int id, int parentId, int order, ref int stepId, ref int transitionId)
        {
            var workflow = new Workflow
            {
                Id = id,
                Name = "Invoice Payable Workflow",
                Description = "Invoice Processing and Payment Approval",
                ParentWorkflowId = parentId,
                Order = order,
                Steps = new List<WorkflowStep>(),
                Transitions = new List<WorkflowTransition>()
            };

            var steps = new[]
            {
                new { Name = "System Approval", Action = ActionType.SystemApproval, Role = Roles.User, RoleName = "System", Actor = "Automated System" },
                new { Name = "User Invoice Processing", Action = ActionType.InvoiceProcessing, Role = Roles.User, RoleName = "Proc Section Head non metal", Actor = "Moh Soliman" },
                new { Name = "Manager Invoice Approval", Action = ActionType.ManagerApproval, Role = Roles.Manager, RoleName = "Procurement Manager", Actor = "Ahmed Youssef" },
                new { Name = "Director Final Approval", Action = ActionType.FinalApproval, Role = Roles.Director, RoleName = "S. Chain Director", Actor = "Manal Adam" }
            };

            for (int i = 0; i < steps.Length; i++)
            {
                var step = new WorkflowStep
                {
                    Id = stepId++,
                    Name = $"Invoice {steps[i].Name}",
                    Order = i + 1,
                    stepStatus = Status.Pending,
                    stepActionTypes = steps[i].Action,
                    isEndStep = i == steps.Length - 1,
                    AssignedRole = steps[i].Role,
                    WorkflowId = id,
                    Roles = new List<StepRole>
                    {
                        new StepRole { Id = stepId, StepId = stepId - 1, RoleName = steps[i].RoleName, ActorName = steps[i].Actor, IsMandatory = true }
                    }
                };
                workflow.Steps.Add(step);
            }

            for (int i = 0; i < workflow.Steps.Count - 1; i++)
            {
                workflow.Transitions.Add(CreateTransition(
                    transitionId++,
                    id,
                    workflow.Steps.ElementAt(i).Id,
                    workflow.Steps.ElementAt(i + 1).Id,
                    workflow.Steps.ElementAt(i).stepActionTypes
                ));
            }

            return workflow;
        }

        private static WorkflowTransition CreateTransition(int id, int workflowId, int fromStepId, int toStepId, ActionType action)
        {
            return new WorkflowTransition
            {
                Id = id,
                WorkflowId = workflowId,
                FromStepId = fromStepId,
                ToStepId = toStepId,
                Action = action,
                FromState = Status.Pending,
                ToState = Status.Pending,
                Timestamp = DateTime.UtcNow,
                PerformedBy = "System"
            };
        }
    }
}
