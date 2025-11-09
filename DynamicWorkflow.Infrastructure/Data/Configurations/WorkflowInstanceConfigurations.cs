using DynamicWorkflow.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DynamicWorkflow.Infrastructure.Data.Configurations
{
    public class WorkflowInstanceConfigurations : IEntityTypeConfiguration<WorkflowInstance>

    {
        public void Configure(EntityTypeBuilder<WorkflowInstance> builder)
        {
            builder.ToTable("WorkflowInstances").HasKey(t => t.Id);
            //relationship instances have the same workflow
            builder.HasOne(ins => ins.Workflow)
                .WithMany(wf => wf.Instances)
                .HasForeignKey(t => t.WorkflowId)
                .OnDelete(DeleteBehavior.Restrict);
            //relationship one step may have many instances
            builder.HasOne(ins => ins.CurrentStep)
                .WithMany().HasForeignKey(f=>f.CurrentStepId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
