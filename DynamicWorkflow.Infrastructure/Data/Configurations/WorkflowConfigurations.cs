using DynamicWorkflow.Core.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWorkflow.Infrastructure.Data.Configurations
{
    public class WorkflowConfigurations : IEntityTypeConfiguration<Workflow>
    {
        public void Configure(EntityTypeBuilder<Workflow> builder)
        {
            builder.ToTable("Workflows");
            builder.HasKey(wfkey => wfkey.Id);
            //WorkflowName Property
            builder.Property(wf=>wf.Name).IsRequired().HasMaxLength(200);
            //Description Property
            builder.Property(wfd=>wfd.Description).HasMaxLength(1000);
            //CreatedAt Property
            builder.Property(wf => wf.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            //Relationships
            //Workflow=>Many Steps 
            builder.HasMany(wf=>wf.Steps)
                .WithOne(s=>s.workflow)
                .HasForeignKey(wfk=>wfk.WorkflowId)
                .OnDelete(DeleteBehavior.Restrict);
            //one Workflow has Many Transitions 
            builder.HasMany(wft=>wft.Transitions)
                .WithOne(wf=>wf.Workflow)
                .HasForeignKey(wfk=>wfk.WorkflowId)
                .OnDelete(DeleteBehavior.Restrict);
            //one Workflow has Many Instances
            builder.HasMany(wf => wf.Instances)
                .WithOne(ins => ins.Workflow)
                .HasForeignKey(wfi => wfi.WorkflowId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(w => w.WorkflowStatus)
       .WithMany()
       .HasForeignKey(w => w.WorkflowStatusId)
       .OnDelete(DeleteBehavior.NoAction);

        }
    }
}
