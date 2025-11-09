//using DynamicWorkflow.Core.Entities;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace DynamicWorkflow.Infrastructure.Data.Configurations
//{
//    public class WorkflowInstanceActionConfigurations : IEntityTypeConfiguration<WorkflowInstanceAction>
//    {

//        public void Configure(EntityTypeBuilder<WorkflowInstanceAction> builder)
//        {
//            builder.ToTable("WorkflowInstanceActions");
//            //Workflow instance has many =>workflow instance action 
//            builder.HasKey(x => x.Id);
//            builder.HasOne(wfiaction => wfiaction.WorkflowInstance).WithMany(ins => ins.WorkflowInstanceActions).HasForeignKey(fk => fk.WorkflowInstanceId)
//                .OnDelete(DeleteBehavior.NoAction);
//            //workflowinstancestep=>has one instance action 
//            builder.HasOne(wfia => wfia.WorkFlowInstanceStep).WithOne(wfis => wfis.WorkflowInstanceAction)
//                .HasForeignKey<WorkflowInstanceAction>(wfi => wfi.WorkFlowInstanceStepId);
                
//            //
//            builder.HasOne(wfi=>wfi.workflowStatus)
//                .WithMany(ws=>ws.WorkflowInstanceActions).HasForeignKey(k=>k.WorkflowStatusId).OnDelete(DeleteBehavior.NoAction);

//        }
        


//    }
//}