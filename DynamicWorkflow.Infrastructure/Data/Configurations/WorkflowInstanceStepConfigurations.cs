using DynamicWorkflow.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWorkflow.Infrastructure.Data.Configurations
{
    public class WorkflowInstanceStepConfigurations : IEntityTypeConfiguration<WorkFlowInstanceStep>
    {
        public void Configure(EntityTypeBuilder<WorkFlowInstanceStep> builder)
            {
                builder.ToTable("WorkFlowInstanceSteps");
                builder.HasKey(isd => isd.Id);

                // Properties
                builder.Property(isd => isd.PerformedByUserId)
                    .HasMaxLength(500);

                builder.Property(isd => isd.Status)
                    .HasMaxLength(50)
                    .IsRequired()
                    .HasDefaultValue("Pending");

                builder.Property(isd => isd.Comments)
                    .HasMaxLength(2000);

                builder.Property(isd => isd.CompletedAt)
                    .IsRequired(false);

                // Relationships

                // One Instance => Many InstanceSteps
                builder.HasOne(isd => isd.Instance)
                    .WithMany() 
                    .HasForeignKey(isd => isd.InstanceId)
                    .OnDelete(DeleteBehavior.Restrict);

                // One Step → Many InstanceSteps
                builder.HasOne(isd => isd.Step)
                    .WithMany(s => s.InstanceSteps)
                    .HasForeignKey(isd => isd.StepId)
                    .OnDelete(DeleteBehavior.Restrict);
            //one instance step =>has one instance action 
            builder.HasOne(wfia => wfia.WorkflowInstanceAction).WithOne(wfis => wfis.WorkFlowInstanceStep)
                .HasForeignKey<WorkflowInstanceAction>(fk => fk.WorkFlowInstanceStepId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
