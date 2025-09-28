using DynamicWorkflow.Core.Entities.Users;
using DynamicWorkflow.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DynamicWorkflow.Core.Entities;
using Microsoft.IdentityModel.Tokens;

namespace DynamicWorkflow.Infrastructure.DataSeeding
{
    public static class ApplicationdbcontextSeed
    {
        public static async Task SeedDataAsync(IServiceProvider services)
        {

            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.RoleManager<ApplicationRole>>();

            await context.Database.MigrateAsync();//ensure database exist and last version  

            // 1. Roles
            string[] RoleNames = { "Admin", "Manager", "Employee", "HR" };
            foreach (var roleName in RoleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new ApplicationRole { Name = roleName });
                }
            }

            // 2. Departments
            var itDept = await context.Departments.FirstOrDefaultAsync(d => d.Name == "IT");
            if (itDept == null)
            {
                itDept = new Department { Id = Guid.NewGuid(), Name = "IT" };
                context.Departments.Add(itDept);
            }

            var financeDept = await context.Departments.FirstOrDefaultAsync(d => d.Name == "Finance");
            if (financeDept == null)
            {
                financeDept = new Department { Id = Guid.NewGuid(), Name = "Finance" };
                context.Departments.Add(financeDept);
            }

            var hrDept = await context.Departments.FirstOrDefaultAsync(d => d.Name == "HR");
            if (hrDept == null)
            {
                hrDept = new Department { Id = Guid.NewGuid(), Name = "HR" };
                context.Departments.Add(hrDept);
            }

            await context.SaveChangesAsync();
            // 3. Manager
            var managerEmail = "manager@sys.com";
            if (await userManager.FindByEmailAsync(managerEmail) == null)
            {
                var manager = new ApplicationUser
                {
                    UserName = managerEmail,
                    Email = managerEmail,
                    NormalizedEmail = managerEmail.ToUpper(),
                    NormalizedUserName = managerEmail.ToUpper(),
                    DisplayName = "System Manager",
                    RegisteredAt = DateTime.UtcNow,
                    DepartmentId = itDept.Id
                };
                await userManager.CreateAsync(manager, "Manager@123");
                await userManager.AddToRoleAsync(manager, "Manager");
            }

            // 4. Employee
            var empEmail = "employee@sys.com";
            if (await userManager.FindByEmailAsync(empEmail) == null)
            {
                var employee = new ApplicationUser
                {
                    UserName = empEmail,
                    Email = empEmail,
                    NormalizedEmail = empEmail.ToUpper(),
                    NormalizedUserName = empEmail.ToUpper(),
                    DisplayName = "Regular Employee",
                    RegisteredAt = DateTime.UtcNow,
                    DepartmentId = itDept.Id
                };
                await userManager.CreateAsync(employee, "Employee@123");
                await userManager.AddToRoleAsync(employee, "Employee");
            }

            // 5. Workflow
            if (!context.Workflows.Any())
            {
                var wf = new Workflow
                {
                    name = "Vacation Request",
                    description = "Workflow for employee vacation requests"
                };
                context.Workflows.Add(wf);
                await context.SaveChangesAsync();

                var step1 = new WorkflowStep { stepName = "Submit Request", WorkflowId = wf.Id, AssignedRole = Core.Enums.Roles.Employee };
                var step2 = new WorkflowStep { stepName = "Manager Approval", WorkflowId = wf.Id, AssignedRole = Core.Enums.Roles.Manager };
                var step3 = new WorkflowStep { stepName = "HR Confirmation", WorkflowId = wf.Id, AssignedRole = Core.Enums.Roles.HR, isEndStep = true };

                context.WorkflowSteps.AddRange(step1, step2, step3);
                await context.SaveChangesAsync();

                context.WorkflowTransitions.AddRange(
                    new WorkflowTransition { FromStep = step1, ToStep = step2, WorkflowId = wf.Id },
                    new WorkflowTransition { FromStep = step2, ToStep = step3, WorkflowId = wf.Id }
                );
                await context.SaveChangesAsync();
            }


        }
    }
}






