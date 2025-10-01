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
    public class StepConfigurations : IEntityTypeConfiguration<WorkflowStep>
    {
        public void Configure(EntityTypeBuilder<WorkflowStep> builder)
        {
            builder.ToTable("WorkflowSteps").HasKey(spk=>spk.Id);
            //property StepName
            builder.Property(s=>s.Name).IsRequired().HasMaxLength(200);
            //Property Comments
            builder.Property(s=>s.Comments).HasMaxLength(2000);
            //Property Step Status StepStatus[Enum]
            builder.Property(s => s.stepStatus).HasConversion<int>().IsRequired();
            //property Assigned Role[Enum]
            builder.Property(s=>s.AssignedRole).HasConversion<int>().IsRequired();
            //Property Action Type[Enum]
            builder.Property(s=>s.stepActionTypes).HasConversion<int>().IsRequired();
            builder.Property(s=>s.isEndStep).IsRequired();
            //relationships
            //One Workflow Has Many Steps
            builder.HasOne(s=>s.workflow)
                .WithMany(w=>w.Steps)
                .HasForeignKey(s=>s.WorkflowId)
                .OnDelete(DeleteBehavior.Cascade);
            //each Step has Many WorkflowInstances
            builder.HasMany(st=>st.InstanceSteps)
                .WithOne(ins=>ins.Step)
                .HasForeignKey(s=>s.StepId)
                .OnDelete(DeleteBehavior.Cascade);
            //each step may have many transitions fromstep
            builder.HasMany(st=>st.OutgoingTransitions)
                .WithOne(tr=>tr.FromStep)
                .HasForeignKey(sfk=>sfk.FromStepId)
                .OnDelete(DeleteBehavior.Restrict);
            //each step may have many transitions tostep
            builder.HasMany(st => st.IncomingTransitions)
                .WithOne(tr => tr.ToStep)
                .HasForeignKey(sfk => sfk.ToStepId);
        }
    }
}
