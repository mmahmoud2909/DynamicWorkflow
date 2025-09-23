using DynamicWorkflow.Core.Entities;
using DynamicWorkflow.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWorkflow.Infrastructure.Data.Configurations
{
    public class WorkflowInstanceConfigurations : IEntityTypeConfiguration<WorkflowInstance>

    {
        public void Configure(EntityTypeBuilder<WorkflowInstance> builder)
        {
            builder.ToTable("WorkflowInstances").HasKey(t => t.Id);
            //Property Instance Status[Enum]
            builder.Property(i => i.State).HasConversion<int>()  // store enum as int
               .HasDefaultValue(Status.Pending)
               .IsRequired();
            //relationship instances have the same workflow
            builder.HasOne(ins => ins.Workflow)
                .WithMany(wf => wf.Instances)
                .HasForeignKey(t => t.WorkflowId)
                .OnDelete(DeleteBehavior.Cascade);
            //relationship one step may have many instances
            builder.HasOne(ins => ins.CurrentStep)
                .WithMany().HasForeignKey(f=>f.CurrentStepId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
