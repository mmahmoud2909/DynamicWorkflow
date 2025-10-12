using DynamicWorkflow.Core.Entities;
using DynamicWorkflow.Core.Enums;

namespace DynamicWorkflow.Infrastructure.DataSeeding
{
    public static class ServiceRepairWorkflowSeedData
    {
        public static List<Workflow> GetWorkflows()
        {
            var workflows = new List<Workflow>();

            // Parent workflow
            var serviceRepair = new Workflow
            {
                Name = "ServiceRepairProcurement",
                Description = "Parent workflow for Service/Repair Procurement process",
                Order = 1
            };
            workflows.Add(serviceRepair);

            // Child workflows
            var prReceiving = CreatePRReceivingWorkflow(serviceRepair, 1);
            workflows.Add(prReceiving);

            var providerSelection = CreateProviderSelectionWorkflow(serviceRepair, 2);
            workflows.Add(providerSelection);

            var poIssuance = CreatePOIssuanceWorkflow(serviceRepair, 3);
            workflows.Add(poIssuance);

            var inbound = CreateInboundWorkflow(serviceRepair, 4);
            workflows.Add(inbound);

            var payment = CreatePaymentWorkflow(serviceRepair, 5);
            workflows.Add(payment);

            var vendorRegistration = CreateVendorRegistrationWorkflow(serviceRepair, 6);
            workflows.Add(vendorRegistration);

            return workflows;
        }

        // -------------------- CHILD 1 --------------------
        private static Workflow CreatePRReceivingWorkflow(Workflow parent, int order)
        {
            var wf = new Workflow
            {
                Name = "PR_ReceivingAndReview_Workflow",
                Description = "Handles PR allocation, review, and data completeness check",
                ParentWorkflowId = parent.Id,
                Order = order,
                Status=Status.Pending,
                Steps = new List<WorkflowStep>()
                
                
            };
            
            wf.Steps.AddRange(new[]
            {
                new WorkflowStep { Name = "Allocate_PR_To_MRO", Order = 1, stepStatus = Status.Pending, stepActionTypes = ActionType.AssignToMRO, AssignedRole = Roles.Procurement },
                new WorkflowStep { Name = "Review_Service_Description", Order = 2, stepStatus = Status.Pending, stepActionTypes = ActionType.ReviewServiceDesc, AssignedRole = Roles.Procurement },
                new WorkflowStep { Name = "Check_PR_Data_Completeness", Order = 3, stepStatus = Status.Pending, stepActionTypes = ActionType.ValidatePRData, Condition = "PR_Data_Missing?", AssignedRole = Roles.Procurement },
                new WorkflowStep { Name = "Return_PR_To_Requester", Order = 4, stepStatus = Status.Pending, stepActionTypes = ActionType.ReturnPR, AssignedRole = Roles.Employee },
                new WorkflowStep { Name = "Wait_For_Resubmission", Order = 5, stepStatus = Status.Pending, stepActionTypes = ActionType.ResubmitPR, AssignedRole = Roles.Employee },
                new WorkflowStep { Name = "Fill_Needed_Info", Order = 6, stepStatus = Status.Pending, stepActionTypes = ActionType.AddInfoAttachments, AssignedRole = Roles.Employee },
                new WorkflowStep { Name = "Review_Service_Description_Again", Order = 7, stepStatus = Status.Pending, stepActionTypes = ActionType.ReviewServiceDesc, AssignedRole = Roles.Procurement, isEndStep = true }
            });

            return wf;
        }
        
        // -------------------- CHILD 2 --------------------
        private static Workflow CreateProviderSelectionWorkflow(Workflow parent, int order)
        {
            var wf = new Workflow
            {
                Name = "ServiceProvidersSelection_Workflow",
                Description = "Handles RFQ, quotation collection, and evaluations",
                ParentWorkflowId = parent.Id,
                Order = order,
                Steps = new List<WorkflowStep>()
            };

            wf.Steps.AddRange(new[]
            {
                new WorkflowStep { Name = "Start_Selection", Order = 1, stepActionTypes = ActionType.StartSelection, stepStatus = Status.Pending, AssignedRole = Roles.Procurement },
                new WorkflowStep { Name = "Check_External_Repair", Order = 2, stepActionTypes = ActionType.CheckExternalType, stepStatus = Status.Pending, Condition = "External_Repair_Calibration?", AssignedRole = Roles.Procurement },
                new WorkflowStep { Name = "Check_Local_Service", Order = 3, stepActionTypes = ActionType.CheckLocalService, stepStatus = Status.Pending, Condition = "Local_Service?", AssignedRole = Roles.Procurement },
                new WorkflowStep { Name = "Check_New_Provider", Order = 4, stepActionTypes = ActionType.CheckNewProvider, stepStatus = Status.Pending, Condition = "New_Service_Provider?", AssignedRole = Roles.Procurement },
                new WorkflowStep { Name = "Fill_RFQ_Form", Order = 5, stepActionTypes = ActionType.CreateRFQ, stepStatus = Status.Pending, AssignedRole = Roles.Procurement },
                new WorkflowStep { Name = "Send_RFQ_To_Providers", Order = 6, stepActionTypes = ActionType.SendRFQ, stepStatus = Status.Pending, AssignedRole = Roles.Procurement },
                new WorkflowStep { Name = "Collect_Quotations", Order = 7, stepActionTypes = ActionType.ReceiveQuotations, stepStatus = Status.Pending, AssignedRole = Roles.Procurement },
                new WorkflowStep { Name = "Verify_Min_Proposals", Order = 8, stepActionTypes = ActionType.VerifyMinProposals, stepStatus = Status.Pending, Condition = "Min_Proposals_Received?", AssignedRole = Roles.Procurement },
                new WorkflowStep { Name = "Technical_Evaluation", Order = 9, stepActionTypes = ActionType.ReviewOffersTech, stepStatus = Status.Pending, AssignedRole = Roles.Technical },
                new WorkflowStep { Name = "Create_Comparison_Sheet", Order = 10, stepActionTypes = ActionType.CreateComparisonSheet, stepStatus = Status.Pending, AssignedRole = Roles.Procurement },
                new WorkflowStep { Name = "Financial_Assessment", Order = 11, stepActionTypes = ActionType.AssessFinancially, stepStatus = Status.Pending, AssignedRole = Roles.Finance },
                new WorkflowStep { Name = "Send_For_Manager_Approval", Order = 12, stepActionTypes = ActionType.SendForManagerApproval, stepStatus = Status.Pending, Condition = "Manager_Approval_Status", AssignedRole = Roles.Manager },
                new WorkflowStep { Name = "Get_SC_Director_Approval", Order = 13, stepActionTypes = ActionType.SeekSCDirectorApproval, stepStatus = Status.Pending, AssignedRole = Roles.Director },
                new WorkflowStep { Name = "Run_Negotiation", Order = 14, stepActionTypes = ActionType.RunNegotiation, stepStatus = Status.Pending, AssignedRole = Roles.Procurement },
                new WorkflowStep { Name = "Send_To_Financial_Approval", Order = 15, stepActionTypes = ActionType.SendForFinancialApproval, stepStatus = Status.Pending, AssignedRole = Roles.Finance, isEndStep = true }
            });

            return wf;
        }

        // -------------------- CHILD 3 --------------------
        private static Workflow CreatePOIssuanceWorkflow(Workflow parent, int order)
        {
            var wf = new Workflow
            {
                Name = "PO_Issuance_Workflow",
                Description = "Purchase Order creation, review, and forwarding",
                ParentWorkflowId = parent.Id,
                Order = order,
                Steps = new List<WorkflowStep>()
            };

            wf.Steps.AddRange(new[]
            {
                new WorkflowStep { Name = "Create_PO", Order = 1, stepActionTypes = ActionType.CreatePO, stepStatus = Status.Pending, AssignedRole = Roles.Procurement },
                new WorkflowStep { Name = "Submit_PO_For_Approval", Order = 2, stepActionTypes = ActionType.SubmitPOApproval, stepStatus = Status.Pending, AssignedRole = Roles.Procurement },
                new WorkflowStep { Name = "Confirm_PO_Approval", Order = 3, stepActionTypes = ActionType.ConfirmPOApproval, stepStatus = Status.Pending, AssignedRole = Roles.Manager },
                new WorkflowStep { Name = "Send_PO_To_Provider", Order = 4, stepActionTypes = ActionType.SendPO, stepStatus = Status.Pending, AssignedRole = Roles.Procurement },
                new WorkflowStep { Name = "Attach_All_Docs", Order = 5, stepActionTypes = ActionType.AttachPODocs, stepStatus = Status.Pending, AssignedRole = Roles.Procurement },
                new WorkflowStep { Name = "Review_Documents_vs_PO", Order = 6, stepActionTypes       = ActionType.ReviewDocuments, stepStatus = Status.Pending, AssignedRole = Roles.Procurement },
                new WorkflowStep { Name = "Sign_Documents", Order = 7, stepActionTypes = ActionType.SignDocuments, stepStatus = Status.Pending, AssignedRole = Roles.Manager },
                new WorkflowStep { Name = "Obtain_SC_Director_Signature", Order = 8, stepActionTypes = ActionType.ObtainDirectorSignature, stepStatus = Status.Pending, AssignedRole = Roles.Director },
                new WorkflowStep { Name = "Forward_PO_To_Logistics", Order = 9, stepActionTypes = ActionType.ForwardToLogistics, stepStatus = Status.Pending, AssignedRole = Roles.Procurement, isEndStep = true }
            });

            return wf;
        }

        // -------------------- CHILD 4 --------------------
        private static Workflow CreateInboundWorkflow(Workflow parent, int order)
        {
            var wf = new Workflow
            {
                Name = "InboundLogistics_Workflow",
                Description = "Inbound logistics process and material receipt",
                ParentWorkflowId = parent.Id,
                Order = order,
                Steps = new List<WorkflowStep>()
            };

            wf.Steps.AddRange(new[]
            {
                new WorkflowStep { Name = "Start_Inbound_Logistics", Order = 1, stepActionTypes = ActionType.StartInboundLogistics, stepStatus = Status.Pending, AssignedRole = Roles.Logistics },
                new WorkflowStep { Name = "Outbound_Logistics_Complete", Order = 2, stepActionTypes = ActionType.CompleteOutbound, stepStatus = Status.Pending, AssignedRole = Roles.Logistics },
                new WorkflowStep { Name = "Ensure_Item_Ready_At_WH", Order = 3, stepActionTypes = ActionType.CheckItemReadiness, stepStatus = Status.Pending, AssignedRole = Roles.Warehouse },
                new WorkflowStep { Name = "Receive_Item_From_Warehouse", Order = 4, stepActionTypes = ActionType.ReceiveItem, stepStatus = Status.Pending, AssignedRole = Roles.Warehouse },
                new WorkflowStep { Name = "Sign_WH_Issue_Voucher", Order = 5, stepActionTypes = ActionType.SignVoucher, stepStatus = Status.Pending, AssignedRole = Roles.Warehouse },
                new WorkflowStep { Name = "Create_Invoice_Form", Order = 6, stepActionTypes = ActionType.CreateInvoiceForm, stepStatus = Status.Pending, AssignedRole = Roles.Finance, isEndStep = true }
            });

            return wf;
        }

        // -------------------- CHILD 5 --------------------
        private static Workflow CreatePaymentWorkflow(Workflow parent, int order)
        {
            var wf = new Workflow
            {
                Name = "SupplierPaymentSettlement_Workflow",
                Description = "Supplier payment settlement and finance confirmation",
                ParentWorkflowId = parent.Id,
                Order = order,
                Steps = new List<WorkflowStep>()
            };

            wf.Steps.AddRange(new[]
            {
                new WorkflowStep { Name = "Start_Payment_Settlement", Order = 1, stepActionTypes = ActionType.StartPaymentProcess, stepStatus = Status.Pending, AssignedRole = Roles.Finance },
                new WorkflowStep { Name = "Review_Payment_Documents", Order = 2, stepActionTypes = ActionType.ReviewPaymentDocs, stepStatus = Status.Pending, AssignedRole = Roles.Finance },
                new WorkflowStep { Name = "Send_Invoice_To_Finance", Order = 3, stepActionTypes = ActionType.SendToFinance, stepStatus = Status.Pending, AssignedRole = Roles.Finance },
                new WorkflowStep { Name = "Confirm_Payment", Order = 4, stepActionTypes = ActionType.ConfirmPayment, stepStatus = Status.Pending, AssignedRole = Roles.Finance, isEndStep = true }
            });

            return wf;
        }

        // -------------------- CHILD 6 (Conditional) --------------------
        private static Workflow CreateVendorRegistrationWorkflow(Workflow parent, int order)
        {
            var wf = new Workflow
            {
                Name = "VendorRegistration_Workflow",
                Description = "Conditional vendor registration workflow for new providers",
                ParentWorkflowId = parent.Id,
                Order = order,
                Steps = new List<WorkflowStep>()
            };

            wf.Steps.AddRange(new[]
            {
                new WorkflowStep { Name = "Start_Vendor_Registration", Order = 1, stepActionTypes = ActionType.StartVendorRegistration, stepStatus = Status.Pending, AssignedRole = Roles.Procurement },
                new WorkflowStep { Name = "Add_Vendor_To_System", Order = 2, stepActionTypes = ActionType.AddVendorToSystem, stepStatus = Status.Pending, AssignedRole = Roles.Procurement },
                new WorkflowStep { Name = "Set_Vendor_Active_Status", Order = 3, stepActionTypes = ActionType.SetActiveStatus, stepStatus = Status.Pending, AssignedRole = Roles.Procurement, isEndStep = true }
            });

            return wf;
        }
    }
}
