
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

            // Main Parent Workflow - LV Plant
            var lvPlantWorkflow = new Workflow
            {
                Name = "LV Plant Workflow",
                Description = "Complete Low Voltage Plant Approval Process",
                ParentWorkflowId = null,
                Order = 1
            };
            workflows.Add(lvPlantWorkflow);

            // Child workflows
            var prApprovalWorkflow = CreatePRApprovalWorkflow(lvPlantWorkflow, 1);
            workflows.Add(prApprovalWorkflow);

            var poApprovalWorkflow = CreatePOApprovalWorkflow(lvPlantWorkflow, 2);
            workflows.Add(poApprovalWorkflow);

            var grnAcceptanceWorkflow = CreateGRNAcceptanceWorkflow(lvPlantWorkflow, 3);
            workflows.Add(grnAcceptanceWorkflow);

            var warehouseWorkflow = CreateWarehouseWorkflow(lvPlantWorkflow, 4);
            workflows.Add(warehouseWorkflow);

            var finalApprovalWorkflow = CreateFinalApprovalWorkflow(lvPlantWorkflow, 5);
            workflows.Add(finalApprovalWorkflow);

            var invoicePayableWorkflow = CreateInvoicePayableWorkflow(lvPlantWorkflow, 6);
            workflows.Add(invoicePayableWorkflow);

            return workflows;
        }

        private static Workflow CreatePRApprovalWorkflow(Workflow parentWorkflow, int order)
        {
            var workflow = new Workflow
            {
                Name = "PR Approval Workflow",
                Description = "Purchase Request Approval Process",
                ParentWorkflowId = parentWorkflow.Id,
                Order = order,
                Steps = new List<WorkflowStep>()
            };

            // Step 1: System Approval
            var systemApproval = new WorkflowStep
            {
                Name = "PR System Approval",
                Order = 1,
                stepStatus = Status.Pending,
                stepActionTypes = ActionType.SystemApproval,
                isEndStep = false,
                AssignedRole = Roles.User,
                Roles = new List<StepRole>
                {
                    new StepRole { RoleName = "System", ActorName = "Automated System", IsMandatory = true }
                }
            };

            // Step 2: User Approval
            var userApproval = new WorkflowStep
            {
                Name = "PR User Approval",
                Order = 2,
                stepStatus = Status.Pending,
                stepActionTypes = ActionType.UserApproval,
                isEndStep = false,
                AssignedRole = Roles.User,
                Roles = new List<StepRole>
                {
                    new StepRole { RoleName = "RM Planning Team Leader", ActorName = "Mariam Shoukry", IsMandatory = true }
                }
            };

            // Step 3: Manager Approval
            var managerApproval = new WorkflowStep
            {
                Name = "PR Manager Approval",
                Order = 3,
                stepStatus = Status.Pending,
                stepActionTypes = ActionType.ManagerApproval,
                isEndStep = false,
                AssignedRole = Roles.Manager,
                Roles = new List<StepRole>
                {
                    new StepRole { RoleName = "Planning Manager", ActorName = "Hany Gawish", IsMandatory = true }
                }
            };

            // Step 4: Director Approval
            var directorApproval = new WorkflowStep
            {
                Name = "PR Director Approval",
                Order = 4,
                stepStatus = Status.Pending,
                stepActionTypes = ActionType.DirectorApproval,
                isEndStep = false,
                AssignedRole = Roles.Director,
                Roles = new List<StepRole>
                {
                    new StepRole { RoleName = "Plant Director", ActorName = "Mohamed Hawary", IsMandatory = true }
                }
            };

            // Step 5: C-Level Approval
            var clevelApproval = new WorkflowStep
            {
                Name = "PR C-Level Approval",
                Order = 5,
                stepStatus = Status.Pending,
                stepActionTypes = ActionType.CLevelApproval,
                isEndStep = true,
                AssignedRole = Roles.CLevel,
                Roles = new List<StepRole>
                {
                    new StepRole { RoleName = "COO", ActorName = "Mohamed Aboud", IsMandatory = true }
                }
            };

            workflow.Steps.Add(systemApproval);
            workflow.Steps.Add(userApproval);
            workflow.Steps.Add(managerApproval);
            workflow.Steps.Add(directorApproval);
            workflow.Steps.Add(clevelApproval);

            return workflow;
        }

        private static Workflow CreatePOApprovalWorkflow(Workflow parentWorkflow, int order)
        {
            var workflow = new Workflow
            {
                Name = "PO Approval Workflow",
                Description = "Purchase Order Approval Process",
                ParentWorkflowId = parentWorkflow.Id,
                Order = order,
                Steps = new List<WorkflowStep>()
            };

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
                    Name = steps[i].Name,
                    Order = i + 1,
                    stepStatus = Status.Pending,
                    stepActionTypes = steps[i].Action,
                    isEndStep = i == steps.Length - 1,
                    AssignedRole = steps[i].Role,
                    Roles = new List<StepRole>
                    {
                        new StepRole { RoleName = steps[i].RoleName, ActorName = steps[i].Actor, IsMandatory = true }
                    }
                };
                workflow.Steps.Add(step);
            }

            return workflow;
        }

        private static Workflow CreateGRNAcceptanceWorkflow(Workflow parentWorkflow, int order)
        {
            var workflow = new Workflow
            {
                Name = "GRN Acceptance Workflow",
                Description = "Goods Received Note Acceptance Process",
                ParentWorkflowId = parentWorkflow.Id,
                Order = order,
                Steps = new List<WorkflowStep>()
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
                    Name = $"GRN {steps[i].Name}",
                    Order = i + 1,
                    stepStatus = Status.Pending,
                    stepActionTypes = steps[i].Action,
                    isEndStep = i == steps.Length - 1,
                    AssignedRole = steps[i].Role,
                    Roles = new List<StepRole>
                    {
                        new StepRole { RoleName = steps[i].RoleName, ActorName = steps[i].Actor, IsMandatory = true }
                    }
                };
                workflow.Steps.Add(step);
            }

            return workflow;
        }

        private static Workflow CreateWarehouseWorkflow(Workflow parentWorkflow, int order)
        {
            var workflow = new Workflow
            {
                Name = "Warehouse Workflow",
                Description = "Warehouse Processing and GRN Generation",
                ParentWorkflowId = parentWorkflow.Id,
                Order = order,
                Steps = new List<WorkflowStep>()
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
                    Name = stepsNonMetal[i].Name,
                    Order = i + 1,
                    stepStatus = Status.Pending,
                    stepActionTypes = stepsNonMetal[i].Action,
                    isEndStep = i == stepsNonMetal.Length - 1,
                    AssignedRole = stepsNonMetal[i].Role,
                    Roles = new List<StepRole>
                    {
                        new StepRole { RoleName = stepsNonMetal[i].RoleName, ActorName = stepsNonMetal[i].Actor, IsMandatory = true }
                    }
                };
                workflow.Steps.Add(step);
            }

            return workflow;
        }

        private static Workflow CreateFinalApprovalWorkflow(Workflow parentWorkflow, int order)
        {
            var workflow = new Workflow
            {
                Name = "Final Approval Workflow",
                Description = "Final System Approval Process",
                ParentWorkflowId = parentWorkflow.Id,
                Order = order,
                Steps = new List<WorkflowStep>()
            };

            var step = new WorkflowStep
            {
                Name = "Final System Approval",
                Order = 1,
                stepStatus = Status.Pending,
                stepActionTypes = ActionType.SystemApproval,
                isEndStep = true,
                AssignedRole = Roles.Director,
                Roles = new List<StepRole>
                {
                    new StepRole { RoleName = "Director", ActorName = "Plant Director", IsMandatory = true }
                }
            };

            workflow.Steps.Add(step);
            return workflow;
        }

        private static Workflow CreateInvoicePayableWorkflow(Workflow parentWorkflow, int order)
        {
            var workflow = new Workflow
            {
                Name = "Invoice Payable Workflow",
                Description = "Invoice Processing and Payment Approval",
                ParentWorkflowId = parentWorkflow.Id,
                Order = order,
                Steps = new List<WorkflowStep>()
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
                    Name = $"Invoice {steps[i].Name}",
                    Order = i + 1,
                    stepStatus = Status.Pending,
                    stepActionTypes = steps[i].Action,
                    isEndStep = i == steps.Length - 1,
                    AssignedRole = steps[i].Role,
                    Roles = new List<StepRole>
                    {
                        new StepRole { RoleName = steps[i].RoleName, ActorName = steps[i].Actor, IsMandatory = true }
                    }
                };
                workflow.Steps.Add(step);
            }

            return workflow;
        }
    }
}