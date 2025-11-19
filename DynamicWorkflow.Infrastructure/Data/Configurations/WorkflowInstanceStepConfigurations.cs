using DynamicWorkflow.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DynamicWorkflow.Infrastructure.Data.Configurations
{
    public class WorkflowInstanceStepConfigurations : IEntityTypeConfiguration<WorkFlowInstanceStep>
    {
        public void Configure(EntityTypeBuilder<WorkFlowInstanceStep> builder)
            {
                builder.ToTable("WorkFlowInstanceSteps");
                builder.HasKey(isd => isd.Id);

                builder.Property(isd => isd.PerformedByUserId)
                    .HasMaxLength(500);

                builder.Property(isd => isd.Comments)
                    .HasMaxLength(2000);

                builder.Property(isd => isd.CompletedAt)
                    .IsRequired(false);

                builder.HasOne(isd => isd.Instance)
                    .WithMany() 
                    .HasForeignKey(isd => isd.InstanceId)
                    .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(isd => isd.Step)
                    .WithMany(s => s.InstanceSteps)
                    .HasForeignKey(isd => isd.StepId)
                    .OnDelete(DeleteBehavior.Restrict);            }
    }
}
