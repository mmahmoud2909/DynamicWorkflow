//using DynamicWorkflow.Core.Entities;
//using DynamicWorkflow.Core.Enums;

//namespace DynamicWorkflow.Infrastructure.DataSeeding
//{
//    public static class ServiceRepairWorkflowSeedData
//    {
//        public static List<Workflow> GetWorkflows()
//        {
//            var workflows = new List<Workflow>();

//            // Parent workflow
//            var serviceRepair = new Workflow
//            {
//                Name = "ServiceRepairProcurement",
//                Description = "Parent workflow for Service/Repair Procurement process",
//                Order = 1
//            };
//            workflows.Add(serviceRepair);

//            // Child workflows
//            workflows.Add(CreatePRReceivingWorkflow(serviceRepair, 1));
//            workflows.Add(CreateProviderSelectionWorkflow(serviceRepair, 2));
//            workflows.Add(CreatePOIssuanceWorkflow(serviceRepair, 3));
//            workflows.Add(CreateInboundWorkflow(serviceRepair, 4));
//            workflows.Add(CreatePaymentWorkflow(serviceRepair, 5));
//            workflows.Add(CreateVendorRegistrationWorkflow(serviceRepair, 6));

//            return workflows;
//        }

//        // -------------------- CHILD 1 --------------------
//        private static Workflow CreatePRReceivingWorkflow(Workflow parent, int order)
//        {
//            var wf = new Workflow
//            {
//                Name = "PR_ReceivingAndReview_Workflow",
//                Description = "Handles PR allocation, review, and data completeness check",
//                ParentWorkflowId = parent.Id,
//                Order = order,
//                Status = Status.Pending,
//                Steps = new List<WorkflowStep>()
//            };

//            wf.Steps.AddRange(new[]
//            {
//                new WorkflowStep { Name = "Allocate_PR_To_MRO", Order = 1,WorkflowStatusId = (int)Status.Pending, stepStatus = Status.Pending, stepActionTypes = ActionType.Accept, ActionTypeEntityId = (int)ActionType.Accept, AssignedRole = Roles.Procurement , AppRoleId = (int)Roles.Procurement },
//                new WorkflowStep { Name = "Review_Service_Description", Order = 2,WorkflowStatusId = (int)Status.Pending, stepStatus = Status.Pending, stepActionTypes = ActionType.Accept, ActionTypeEntityId =(int) ActionType.Accept, AssignedRole = Roles.Procurement  , AppRoleId = (int)Roles.Procurement },
//                new WorkflowStep { Name = "Check_PR_Data_Completeness", Order = 3, WorkflowStatusId = (int)Status.Pending,  stepStatus = Status.Pending, stepActionTypes = ActionType.Accept, ActionTypeEntityId =(int) ActionType.Accept, AssignedRole = Roles.Procurement  , AppRoleId = (int)Roles.Procurement },
//                new WorkflowStep { Name = "Return_PR_To_Requester", Order = 4, WorkflowStatusId = (int)Status.Pending, stepStatus = Status.Pending, stepActionTypes = ActionType.Reject,   ActionTypeEntityId = (int)ActionType.Reject, AssignedRole = Roles.Employee, AppRoleId = (int)Roles.Employee  },
//                new WorkflowStep { Name = "Wait_For_Resubmission", Order = 5, WorkflowStatusId =(int) Status.Pending, stepStatus = Status.Pending, stepActionTypes = ActionType.Accept, ActionTypeEntityId =(int) ActionType.Accept, AssignedRole = Roles.Employee , AppRoleId =(int) Roles.Employee},
//                new WorkflowStep { Name = "Fill_Needed_Info", Order = 6, WorkflowStatusId =(int) Status.Pending, stepStatus = Status.Pending, stepActionTypes = ActionType.Accept, ActionTypeEntityId =(int) ActionType.Accept, AssignedRole = Roles.Employee , AppRoleId =(int) Roles.Employee},
//                new WorkflowStep { Name = "Review_Service_Description_Again", Order = 7, WorkflowStatusId =(int) Status.Pending, stepStatus = Status.Pending, stepActionTypes = ActionType.Accept, ActionTypeEntityId =(int) ActionType.Accept, AssignedRole = Roles.Procurement , AppRoleId = (int)Roles.Procurement , isEndStep = true }
//            });

//            return wf;
//        }

//        // -------------------- CHILD 2 --------------------
//        private static Workflow CreateProviderSelectionWorkflow(Workflow parent, int order)
//        {
//            var wf = new Workflow
//            {
//                Name = "ServiceProvidersSelection_Workflow",
//                Description = "Handles RFQ, quotation collection, and evaluations",
//                ParentWorkflowId = parent.Id,
//                Order = order,
//                Steps = new List<WorkflowStep>()
//            };

//            wf.Steps.AddRange(new[]
//            {
//                new WorkflowStep { Name = "Start_Selection", Order = 1, stepActionTypes = ActionType.Accept, WorkflowStatusId = (int)Status.Pending, stepStatus = Status.Pending, ActionTypeEntityId = (int)ActionType.Accept, AssignedRole = Roles.Procurement  , AppRoleId = (int)Roles.Procurement },
//                new WorkflowStep { Name = "Collect_Quotations", Order = 2, stepActionTypes = ActionType.Accept, WorkflowStatusId =(int) Status.Pending, stepStatus = Status.Pending, ActionTypeEntityId =(int) ActionType.Accept, AssignedRole = Roles.Procurement  , AppRoleId = (int)Roles.Procurement },
//                new WorkflowStep { Name = "Technical_Evaluation", Order = 3, stepActionTypes = ActionType.Accept, WorkflowStatusId =(int) Status.Pending, stepStatus = Status.Pending, ActionTypeEntityId =(int) ActionType.Accept, AssignedRole = Roles.Technical , AppRoleId =(int) Roles.Technical},
//                new WorkflowStep { Name = "Financial_Assessment", Order = 4, stepActionTypes = ActionType.Accept, WorkflowStatusId =(int) Status.Pending, stepStatus = Status.Pending, ActionTypeEntityId =(int) ActionType.Accept, AssignedRole = Roles.Finance , AppRoleId =(int) Roles.Finance},
//                new WorkflowStep { Name = "Manager_Approval", Order = 5, stepActionTypes = ActionType.Accept, WorkflowStatusId =(int) Status.Pending, stepStatus = Status.Pending, ActionTypeEntityId =(int) ActionType.Accept, AssignedRole = Roles.Manager , AppRoleId = (int)Roles.Manager },
//                new WorkflowStep { Name = "Director_Approval", Order = 6, stepActionTypes = ActionType.Accept, WorkflowStatusId =(int) Status.Pending, stepStatus = Status.Pending, ActionTypeEntityId =(int) ActionType.Accept, AssignedRole = Roles.Director, AppRoleId = (int)Roles.Director , isEndStep = true }
//            });

//            return wf;
//        }

//        // -------------------- CHILD 3 --------------------
//        private static Workflow CreatePOIssuanceWorkflow(Workflow parent, int order)
//        {
//            var wf = new Workflow
//            {
//                Name = "PO_Issuance_Workflow",
//                Description = "Purchase Order creation, review, and forwarding",
//                ParentWorkflowId = parent.Id,
//                Order = order,
//                Steps = new List<WorkflowStep>()
//            };

//            wf.Steps.AddRange(new[]
//            {
//                new WorkflowStep { Name = "Create_PO", Order = 1, stepActionTypes = ActionType.Accept, WorkflowStatusId =(int) Status.Pending, stepStatus = Status.Pending, ActionTypeEntityId =(int) ActionType.Accept, AssignedRole = Roles.Procurement  , AppRoleId = (int)Roles.Procurement },
//                new WorkflowStep { Name = "Submit_PO_For_Approval", Order = 2, stepActionTypes = ActionType.Accept, WorkflowStatusId =(int) Status.Pending, stepStatus = Status.Pending, ActionTypeEntityId =(int) ActionType.Accept, AssignedRole = Roles.Manager , AppRoleId = (int)Roles.Manager },
//                new WorkflowStep { Name = "Confirm_PO_Approval", Order = 3, stepActionTypes = ActionType.Accept, WorkflowStatusId =(int) Status.Pending, stepStatus = Status.Pending, ActionTypeEntityId = (int)ActionType.Accept, AssignedRole = Roles.Director, AppRoleId = (int)Roles.Director , isEndStep = true }
//            });

//            return wf;
//        }

//        // -------------------- CHILD 4 --------------------
//        private static Workflow CreateInboundWorkflow(Workflow parent, int order)
//        {
//            var wf = new Workflow
//            {
//                Name = "InboundLogistics_Workflow",
//                Description = "Inbound logistics process and material receipt",
//                ParentWorkflowId = parent.Id,
//                Order = order,
//                Steps = new List<WorkflowStep>()
//            };

//            wf.Steps.AddRange(new[]
//            {
//                new WorkflowStep { Name = "Start_Inbound_Logistics", Order = 1, stepActionTypes = ActionType.Accept, WorkflowStatusId =(int) Status.Pending, stepStatus = Status.Pending, ActionTypeEntityId = (int)ActionType.Accept, AssignedRole = Roles.Logistics, AppRoleId = (int)Roles.Logistics  },
//                new WorkflowStep { Name = "Confirm_Warehouse_Receipt", Order = 2, stepActionTypes = ActionType.Accept, WorkflowStatusId =(int) Status.Pending, stepStatus = Status.Pending, ActionTypeEntityId = (int)ActionType.Accept, AssignedRole = Roles.Warehouse , AppRoleId = (int)Roles.Warehouse  },
//                new WorkflowStep { Name = "Finance_Invoice_Creation", Order = 3, stepActionTypes = ActionType.Accept, WorkflowStatusId =(int) Status.Pending, stepStatus = Status.Pending, ActionTypeEntityId = (int)ActionType.Accept, AssignedRole = Roles.Finance, AppRoleId = (int)Roles.Finance , isEndStep = true }
//            });

//            return wf;
//        }

//        // -------------------- CHILD 5 --------------------
//        private static Workflow CreatePaymentWorkflow(Workflow parent, int order)
//        {
//            var wf = new Workflow
//            {
//                Name = "SupplierPaymentSettlement_Workflow",
//                Description = "Supplier payment settlement and finance confirmation",
//                ParentWorkflowId = parent.Id,
//                Order = order,
//                Steps = new List<WorkflowStep>()
//            };

//            wf.Steps.AddRange(new[]
//            {
//                new WorkflowStep { Name = "Start_Payment_Settlement", Order = 1, stepActionTypes = ActionType.Accept, WorkflowStatusId =(int) Status.Pending, stepStatus = Status.Pending, ActionTypeEntityId = (int)ActionType.Accept, AssignedRole = Roles.Finance , AppRoleId = (int)Roles.Finance  },
//                new WorkflowStep { Name = "Review_Payment_Documents", Order = 2, stepActionTypes = ActionType.Accept, WorkflowStatusId =(int) Status.Pending, stepStatus = Status.Pending, ActionTypeEntityId = (int)ActionType.Accept, AssignedRole = Roles.Finance, AppRoleId = (int)Roles.Finance  },
//                new WorkflowStep { Name = "Confirm_Payment", Order = 3, stepActionTypes = ActionType.Accept, WorkflowStatusId =(int) Status.Pending, stepStatus = Status.Pending,   ActionTypeEntityId = (int)ActionType.Accept, AssignedRole = Roles.Finance, AppRoleId = (int)Roles.Finance , isEndStep = true }
//            });

//            return wf;
//        }

//        // -------------------- CHILD 6 --------------------
//        private static Workflow CreateVendorRegistrationWorkflow(Workflow parent, int order)
//        {
//            var wf = new Workflow
//            {
//                Name = "VendorRegistration_Workflow",
//                Description = "Vendor registration workflow for new providers",
//                ParentWorkflowId = parent.Id,
//                Order = order,
//                Steps = new List<WorkflowStep>()
//            };

//            wf.Steps.AddRange(new[]
//            {
//                new WorkflowStep { Name = "Start_Vendor_Registration", Order = 1, stepActionTypes = ActionType.Accept, ActionTypeEntityId = (int)ActionType.Accept, WorkflowStatusId =(int) Status.Pending, stepStatus = Status.Pending, AssignedRole = Roles.Procurement  , AppRoleId = (int)Roles.Procurement },
//                new WorkflowStep { Name = "Add_Vendor_To_System", Order = 2, stepActionTypes = ActionType.Accept, ActionTypeEntityId = (int)ActionType.Accept, WorkflowStatusId =(int) Status.Pending, stepStatus = Status.Pending, AssignedRole = Roles.Procurement  , AppRoleId = (int)Roles.Procurement },
//                new WorkflowStep { Name = "Set_Vendor_Active_Status", Order = 3, stepActionTypes = ActionType.Accept, ActionTypeEntityId =(int) ActionType.Accept, WorkflowStatusId =(int) Status.Pending, stepStatus = Status.Pending, AssignedRole = Roles.Procurement , AppRoleId = (int)Roles.Procurement , isEndStep = true }
//            });

//            return wf;
//        }
//    }
//}
