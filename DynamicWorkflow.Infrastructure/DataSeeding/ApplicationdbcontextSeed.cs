using DynamicWorkflow.Core.Entities;
using DynamicWorkflow.Core.Entities.Users;
using DynamicWorkflow.Core.Enums;
using DynamicWorkflow.Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

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

            await context.Database.MigrateAsync();

            // 1️⃣ Identity Roles
            string[] RoleNames = { "Admin", "Manager", "Employee", "HR" };
            foreach (var roleName in RoleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                    await roleManager.CreateAsync(new ApplicationRole { Name = roleName });
            }

            // 2️⃣ Departments
            string[] departments = { "IT", "Finance", "HR" };
            foreach (var deptName in departments)
            {
                if (!await context.Departments.AnyAsync(d => d.Name == deptName))
                    context.Departments.Add(new Department { Id = Guid.NewGuid(), Name = deptName });
            }
            await context.SaveChangesAsync();

            // 3️⃣ Manager User
            var managerEmail = "manager@sys.com";
            if (await userManager.FindByEmailAsync(managerEmail) == null)
            {
                var dept = await context.Departments.FirstAsync(d => d.Name == "IT");
                var manager = new ApplicationUser
                {
                    UserName = managerEmail,
                    Email = managerEmail,
                    NormalizedEmail = managerEmail.ToUpper(),
                    NormalizedUserName = managerEmail.ToUpper(),
                    DisplayName = "System Manager",
                    RegisteredAt = DateTime.UtcNow,
                    DepartmentId = dept.Id
                };
                await userManager.CreateAsync(manager, "Manager@123");
                await userManager.AddToRoleAsync(manager, "Manager");
            }

            // 4️⃣ Employee User
            var empEmail = "employee@sys.com";
            if (await userManager.FindByEmailAsync(empEmail) == null)
            {
                var dept = await context.Departments.FirstAsync(d => d.Name == "IT");
                var employee = new ApplicationUser
                {
                    UserName = empEmail,
                    Email = empEmail,
                    NormalizedEmail = empEmail.ToUpper(),
                    NormalizedUserName = empEmail.ToUpper(),
                    DisplayName = "Regular Employee",
                    RegisteredAt = DateTime.UtcNow,
                    DepartmentId = dept.Id
                };
                await userManager.CreateAsync(employee, "Employee@123");
                await userManager.AddToRoleAsync(employee, "Employee");
            }

            // 5️⃣ Admin User
            var adminEmail = "admin@sys.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var dept = await context.Departments.FirstAsync(d => d.Name == "IT");
                var admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    NormalizedEmail = adminEmail.ToUpper(),
                    NormalizedUserName = adminEmail.ToUpper(),
                    DisplayName = "System Administrator",
                    RegisteredAt = DateTime.UtcNow,
                    DepartmentId = dept.Id
                };

                var result = await userManager.CreateAsync(admin, "Admin@123");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, "Admin");
            }

            // 6️⃣ Action Types
            if (!await context.ActionTypes.AnyAsync())
            {
                var actionTypes = new[]
                {
                    new ActionTypeEntity { Name = "Accept" },
                    new ActionTypeEntity { Name = "Reject" },
                    new ActionTypeEntity { Name = "Hold" }
                };
                await context.ActionTypes.AddRangeAsync(actionTypes);
                await context.SaveChangesAsync();
            }

            // 7️⃣ App Roles
            if (!await context.AppRoles.AnyAsync())
            {
                var appRoles = new[]
                {
                    new AppRole { Name = "Logistics" },
                    new AppRole { Name = "Finance" },
                    new AppRole { Name = "Employee" },
                    new AppRole { Name = "Manager" },
                    new AppRole { Name = "HR" },
                    new AppRole { Name = "User" },
                    new AppRole { Name = "Director" },
                    new AppRole { Name = "CLevel" },
                    new AppRole { Name = "Technical" },
                    new AppRole { Name = "Planning" },
                    new AppRole { Name = "Treasury" },
                    new AppRole { Name = "Procurement" },
                    new AppRole { Name = "Warehouse" },
                    new AppRole { Name = "QC" },
                    new AppRole { Name = "Supervisor" },
                    new AppRole { Name = "StoreKeeper" }
                };

                await context.AppRoles.AddRangeAsync(appRoles);
                await context.SaveChangesAsync();
            }

            // 8️⃣ Workflow Statuses
            if (!await context.WorkflowStatuses.AnyAsync())
            {
                var statuses = new[]
                {
                    new WorkflowStatus { Name = "Pending" },
                    new WorkflowStatus { Name = "InProgress" },
                    new WorkflowStatus { Name = "Accepted" },
                    new WorkflowStatus { Name = "Rejected" },
                    new WorkflowStatus { Name = "Completed" }
                };
                await context.WorkflowStatuses.AddRangeAsync(statuses);
                await context.SaveChangesAsync();
            }
        }

        // Existing workflow seeding logic
        public static IApplicationBuilder SeedWorkflowData(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>();
                context.Database.Migrate();

                if (!context.Workflows.Any())
                {
                    SeedWorkflowsWithRelationships(context);
                    Console.WriteLine("Workflow seed data added successfully!");
                }
            }

            return app;
        }

        private static void ClearExistingData(ApplicationIdentityDbContext context)
        {
            context.WorkflowTransitions.RemoveRange(context.WorkflowTransitions);
            context.StepRoles.RemoveRange(context.StepRoles);
            context.WorkflowSteps.RemoveRange(context.WorkflowSteps);
            context.Workflows.RemoveRange(context.Workflows);
            context.SaveChanges();
        }

private static void SeedWorkflowsWithRelationships(ApplicationIdentityDbContext context)
{
    // Make sure we have at least one WorkflowStatus
    var defaultStatus = context.WorkflowStatuses.FirstOrDefault(s => s.Name == "Pending")
        ?? context.WorkflowStatuses.First(); // fallback

    if (!context.Workflows.Any(w => w.Name == "ServiceRepairProcurement"))
    {
        var workflows = ServiceRepairWorkflowSeedData.GetWorkflows();

        // ✅ Assign a valid WorkflowStatusId to each workflow
        foreach (var wf in workflows)
        {
            wf.WorkflowStatusId = defaultStatus.Id;
        }

        context.Workflows.AddRange(workflows);
        context.SaveChanges();

        Console.WriteLine("✅ ServiceRepairProcurement workflows added successfully!");
    }
    else
    {
        Console.WriteLine("⚠️ ServiceRepairProcurement workflows already exist — skipping seeding.");
    }
}


        private static void CreateTransitionsForWorkflow(Workflow workflow)
        {
            var steps = workflow.Steps.OrderBy(s => s.Order).ToList();

            for (int i = 0; i < steps.Count - 1; i++)
            {
                var transition = new WorkflowTransition
                {
                    WorkflowId = workflow.Id,
                    FromStepId = steps[i].Id,
                    ToStepId = steps[i + 1].Id,
                    Action = steps[i].stepActionTypes,
                    ActionTypeEntityId = (int)steps[i].stepActionTypes,
                    FromState = Status.Pending,
                    ToState = Status.Pending,
                    Timestamp = DateTime.UtcNow,
                    PerformedBy = "System"
                };
            }
        }
    }
}
