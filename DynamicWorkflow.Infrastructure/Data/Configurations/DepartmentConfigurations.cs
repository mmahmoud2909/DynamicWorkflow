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
    public class DepartmentConfigurations : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.ToTable("Departments");
            builder.Property(d=>d.Name).IsRequired().HasMaxLength(200);
            builder.HasMany(d => d.Users).WithOne(u => u.Department).HasForeignKey(u => u.DepartmentId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
