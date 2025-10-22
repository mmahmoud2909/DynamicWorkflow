namespace DynamicWorkflow.Core.Enums
{
    public enum ActionType
    {

        Accept = 1,
        Reject = 2,
        Hold = 3
    } }

        // ===== General / Core Actions =====
        //Create,
        //StartProcess,
        //CompleteProcess,
        //Notify,
        //Skip,
        //RequestChanges,
        //AddInfoAttachments,
        //SystemApproval,

        // ===== Purchase Request (PR) =====
        //CreateRequest,
        //ReviewRequest,
        //AssignToMRO,
        //ReviewServiceDesc,
        //ValidatePRData,
        //ReturnPR,
        //ResubmitPR,

        // ===== Service Provider Selection =====
        //StartSelection,
        //CheckExternalType,
        //CheckLocalService,
        //CheckNewProvider,
        //CreateRFQ,
        //SendRFQ,
        //ReceiveQuotations,
        //VerifyMinProposals,
        //ReviewOffersTech,
        //MarkRejectedQuotations,
        //CreateComparisonSheet,
        //AssessFinancially,
        //SendForManagerApproval,
        //ProcessManagerApproval,
        //SendNotesToProvider,
        //DiscardAndFeedback,
        //SeekSCDirectorApproval,
        //ShareNomination,
        //RunNegotiation,
        //SendForFinalTechApproval,
        //ObtainManagerApproval,
        //SendForFinancialApproval,

        //// ===== Purchase Order (PO) =====
        //CreatePO,
        //SubmitPOApproval,
        //ConfirmPOApproval,
        //SendPO,
        //AttachPODocs,
        //ReviewDocuments,
        //SignDocuments,
        //ObtainDirectorSignature,
        //ForwardToLogistics,

        //// ===== Inbound Logistics =====
        //StartInboundLogistics,
        //CompleteOutbound,
        //CheckItemReadiness,
        //ReceiveItem,
        //SignVoucher,
        //CreateInvoiceForm,

        //// ===== Payment Settlement =====
        //StartPaymentProcess,
        //ReviewPaymentDocs,
        //SendToFinance,
        //ConfirmPayment,

        //// ===== Vendor Registration =====
        //StartVendorRegistration,
        //AddVendorToSystem,
        //SetActiveStatus,

        //// ===== Approvals (Generic) =====
        //UserApproval,
        //ManagerApproval,
        //DirectorApproval,
        //CLevelApproval,
        //TechnicalApproval,
        //PlanningApproval,
        //TreasuryApproval,
        //ProcurementApproval,
        //WarehouseApproval,
        //QcApproval,
        //FinalApproval,
        //GrnGeneration,
        //InvoiceProcessing