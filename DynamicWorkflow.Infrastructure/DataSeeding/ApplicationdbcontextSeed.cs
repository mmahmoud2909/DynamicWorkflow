using DynamicWorkflow.Core.Entities;
using DynamicWorkflow.Core.Entities.Users;
using DynamicWorkflow.Core.Enums;
using DynamicWorkflow.Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DynamicWorkflow.Infrastructure.DataSeeding
{
    public static class ApplicationdbcontextSeed
    {
        
        public static async Task SeedDataAsync(IServiceProvider services)
        {

            using var scope = services.CreateScope();//defines the services i want to get and already registed in my program
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

            // 6. Admin
            var adminEmail = "admin@sys.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    NormalizedEmail = adminEmail.ToUpper(),
                    NormalizedUserName = adminEmail.ToUpper(),
                    DisplayName = "System Administrator",
                    RegisteredAt = DateTime.UtcNow,
                    DepartmentId = itDept.Id
                };

                var result = await userManager.CreateAsync(admin, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }
        }

        public static IApplicationBuilder SeedWorkflowData(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>();
                context.Database.Migrate();

                // Clear existing data
               // ClearExistingData(context);

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
            // Clear in correct order to respect foreign key constraints
            context.WorkflowTransitions.RemoveRange(context.WorkflowTransitions);
            context.StepRoles.RemoveRange(context.StepRoles);
            context.WorkflowSteps.RemoveRange(context.WorkflowSteps);
            context.Workflows.RemoveRange(context.Workflows);

            context.SaveChanges();
        }

        private static void SeedWorkflowsWithRelationships(ApplicationIdentityDbContext context)
        {
            // ✅ Add only ServiceRepairProcurement workflows if not already added
            if (!context.Workflows.Any(w => w.Name == "ServiceRepairProcurement"))
            {
                var serviceWorkflows = ServiceRepairWorkflowSeedData.GetWorkflows();
                context.Workflows.AddRange(serviceWorkflows);
                context.SaveChanges();

                Console.WriteLine("✅ ServiceRepairProcurement workflows added successfully!");
            }
            else
            {
                Console.WriteLine("⚠️ ServiceRepairProcurement workflows already exist — skipping seeding.");
            }
        }
        
       // var workflows = WorkflowSeedData.GetWorkflows();
            // Add workflows first to get their IDs
        //    context.Workflows.AddRange(workflows);
        //    context.SaveChanges();

        //    // Now create transitions after all steps have been saved and have IDs
        //    foreach (var workflow in workflows)
        //    {
        //        CreateTransitionsForWorkflow(workflow);
        //    }

        //    context.SaveChanges();
        //}

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
                    FromState = Status.Pending,
                    ToState = Status.Pending,
                    Timestamp = DateTime.UtcNow,
                    PerformedBy = "System"
                };
            }
        }
    }
}