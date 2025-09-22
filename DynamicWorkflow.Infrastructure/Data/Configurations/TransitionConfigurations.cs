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
    public class TransitionConfigurations : IEntityTypeConfiguration<WorkflowTransition>
    {
        public void Configure(EntityTypeBuilder<WorkflowTransition> builder)
        {
            builder.ToTable("WorkflowTransitions");
            builder.HasKey(t => t.Id);

            // Relationships
            // One Workflow=> Many Transitions
            builder.HasOne(t => t.workflow)
                .WithMany(w => w.Transitions)
                .HasForeignKey(t => t.WorkflowId)
                .OnDelete(DeleteBehavior.Cascade);

            // FromStep => OutgoingTransitions
            builder.HasOne(t => t.FromStep)
                .WithMany(s => s.OutgoingTransitions)
                .HasForeignKey(t => t.FromStepId)
                .OnDelete(DeleteBehavior.Restrict);
            // ToStep => IncomingTransitions
            builder.HasOne(t => t.ToStep)
                .WithMany(s => s.IncomingTransitions)
                .HasForeignKey(t => t.ToStepId)
                .OnDelete(DeleteBehavior.Restrict);
            

        }
    }
}
