using DynamicWorkflow.Core.Entities;
using DynamicWorkflow.Core.Entities.Users;
using DynamicWorkflow.Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DynamicWorkflow.Infrastructure.DataSeeding
{
    public static class ApplicationDbContextSeed
    {
        public static async Task SeedDataAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

            await context.Database.MigrateAsync();

            await SeedRolesAsync(roleManager);

            await SeedDepartmentsAsync(context);

            await SeedUsersAsync(context, userManager);

            await SeedWorkflowStatusesAsync(context);

            await SeedActionTypesAsync(context);

            await SeedAppRolesAsync(context);

            await SeedWorkflowsAsync(context);

            Console.WriteLine("✅ All seed data added successfully!");
        }

        private static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager)
        {
            string[] roleNames = { "Admin", "Manager", "Employee", "Procurement", "Finance", "Technical", "Warehouse", "Logistics", "HR", "Planning" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new ApplicationRole { Name = roleName });
                }
            }
        }

        private static async Task SeedDepartmentsAsync(ApplicationIdentityDbContext context)
        {
            string[] departments = { "IT", "Finance", "Procurement", "Technical", "Warehouse", "Logistics", "HR", "Planning" };

            foreach (var deptName in departments)
            {
                if (!await context.Departments.AnyAsync(d => d.Name == deptName))
                {
                    context.Departments.Add(new Department
                    {
                        Id = Guid.NewGuid(),
                        Name = deptName
                    });
                }
            }
            await context.SaveChangesAsync();
        }

        private static async Task SeedUsersAsync(ApplicationIdentityDbContext context, UserManager<ApplicationUser> userManager)
        {
            var departmentUsers = new[]
            {
                new { Email = "ahmed.samir@sys.com", Display = "Ahmed Samir", Role = "Procurement", Dept = "Procurement" },
                new { Email = "mariam.shoukry@sys.com", Display = "Mariam Shoukry", Role = "Finance", Dept = "Finance" },
                new { Email = "hany.gawish@sys.com", Display = "Hany Gawish", Role = "Technical", Dept = "Technical" },
                new { Email = "mostafa.hussein@sys.com", Display = "Mostafa Hussein", Role = "Warehouse", Dept = "Warehouse" },
                new { Email = "sara.adel@sys.com", Display = "Sara Adel", Role = "Logistics", Dept = "Logistics" },
                new { Email = "nouran.hassan@sys.com", Display = "Nouran Hassan", Role = "HR", Dept = "HR" },
                new { Email = "tarek.mostafa@sys.com", Display = "Tarek Mostafa", Role = "Planning", Dept = "Planning" },
                new { Email = "admin@sys.com", Display = "System Administrator", Role = "Admin", Dept = "IT" },
                new { Email = "manager@sys.com", Display = "System Manager", Role = "Manager", Dept = "IT" },
                new { Email = "employee@sys.com", Display = "Regular Employee", Role = "Employee", Dept = "IT" }
            };

            foreach (var u in departmentUsers)
            {
                if (await userManager.FindByEmailAsync(u.Email) == null)
                {
                    var dept = await context.Departments.FirstAsync(d => d.Name == u.Dept);
                    var user = new ApplicationUser
                    {
                        UserName = u.Email,
                        Email = u.Email,
                        NormalizedEmail = u.Email.ToUpper(),
                        NormalizedUserName = u.Email.ToUpper(),
                        DisplayName = u.Display,
                        RegisteredAt = DateTime.UtcNow,
                        DepartmentId = dept.Id
                    };

                    var result = await userManager.CreateAsync(user, "Pass@123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, u.Role);
                    }
                }
            }
        }

        private static async Task SeedWorkflowStatusesAsync(ApplicationIdentityDbContext context)
        {
            if (!await context.WorkflowStatuses.AnyAsync())
            {
                var statuses = new[]
                {
                    new WorkflowStatus { Name = "Pending", Description = "Workflow or step is pending" },
                    new WorkflowStatus { Name = "InProgress", Description = "Workflow or step is in progress" },
                    new WorkflowStatus { Name = "Accepted", Description = "Workflow or step was accepted" },
                    new WorkflowStatus { Name = "Rejected", Description = "Workflow or step was rejected" },
                    new WorkflowStatus { Name = "Completed", Description = "Workflow or step is completed" },
                    new WorkflowStatus { Name = "Cancelled", Description = "Workflow or step was cancelled" }
                };

                await context.WorkflowStatuses.AddRangeAsync(statuses);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedActionTypesAsync(ApplicationIdentityDbContext context)
        {
            if (!await context.ActionTypes.AnyAsync())
            {
                var actionTypes = new[]
                {
                    new ActionTypeEntity { Name = "Accept", Description = "Accept action" },
                    new ActionTypeEntity { Name = "Reject", Description = "Reject action" },
                    new ActionTypeEntity { Name = "Hold", Description = "Hold action" },
                    new ActionTypeEntity { Name = "Cancel", Description = "Cancel action" },
                    new ActionTypeEntity { Name = "Restart", Description = "Restart action" }
                };

                await context.ActionTypes.AddRangeAsync(actionTypes);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedAppRolesAsync(ApplicationIdentityDbContext context)
        {
            if (!await context.AppRoles.AnyAsync())
            {
                var appRoles = new[]
                {
                    new AppRole { Name = "Logistics", Description = "Logistics department role" },
                    new AppRole { Name = "Finance", Description = "Finance department role" },
                    new AppRole { Name = "Employee", Description = "General employee role" },
                    new AppRole { Name = "Manager", Description = "Manager role" },
                    new AppRole { Name = "HR", Description = "Human Resources role" },
                    new AppRole { Name = "User", Description = "General user role" },
                    new AppRole { Name = "Director", Description = "Director role" },
                    new AppRole { Name = "CLevel", Description = "C-Level executive role" },
                    new AppRole { Name = "Technical", Description = "Technical department role" },
                    new AppRole { Name = "Planning", Description = "Planning department role" },
                    new AppRole { Name = "Treasury", Description = "Treasury department role" },
                    new AppRole { Name = "Procurement", Description = "Procurement department role" },
                    new AppRole { Name = "Warehouse", Description = "Warehouse department role" },
                    new AppRole { Name = "QC", Description = "Quality Control role" },
                    new AppRole { Name = "Supervisor", Description = "Supervisor role" },
                    new AppRole { Name = "StoreKeeper", Description = "Store Keeper role" }
                };

                await context.AppRoles.AddRangeAsync(appRoles);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedWorkflowsAsync(ApplicationIdentityDbContext context)
        {
            if (!await context.Workflows.AnyAsync())
            {
                var pendingStatus = await context.WorkflowStatuses.FirstAsync(ws => ws.Name == "Pending");
                var acceptAction = await context.ActionTypes.FirstAsync(a => a.Name == "Accept");
                var rejectAction = await context.ActionTypes.FirstAsync(a => a.Name == "Reject");

                var appRoles = await context.AppRoles.ToDictionaryAsync(ar => ar.Name, ar => ar.Id);

                var parentWorkflow = new Workflow
                {
                    Name = "ServiceRepairProcurement",
                    Description = "Parent workflow for Service/Repair Procurement process",
                    Order = 1,
                    WorkflowStatusId = pendingStatus.Id,
                    CreatedBy = "System",
                    CreatedAt = DateTime.UtcNow
                };

                context.Workflows.Add(parentWorkflow);
                await context.SaveChangesAsync();

                var childWorkflows = new[]
                {
                    CreatePRReceivingWorkflow(parentWorkflow, 1, pendingStatus, acceptAction, rejectAction, appRoles),
                    CreateProviderSelectionWorkflow(parentWorkflow, 2, pendingStatus, acceptAction, appRoles),
                    CreatePOIssuanceWorkflow(parentWorkflow, 3, pendingStatus, acceptAction, appRoles),
                    CreateInboundWorkflow(parentWorkflow, 4, pendingStatus, acceptAction, appRoles),
                    CreatePaymentWorkflow(parentWorkflow, 5, pendingStatus, acceptAction, appRoles),
                    CreateVendorRegistrationWorkflow(parentWorkflow, 6, pendingStatus, acceptAction, appRoles)
                };

                context.Workflows.AddRange(childWorkflows);
                await context.SaveChangesAsync();

                Console.WriteLine("✅ ServiceRepairProcurement workflows added successfully!");
            }
            else
            {
                Console.WriteLine("⚠️ Workflows already exist — skipping seeding.");
            }
        }

        private static Workflow CreatePRReceivingWorkflow(Workflow parent, int order, WorkflowStatus status,
            ActionTypeEntity acceptAction, ActionTypeEntity rejectAction, Dictionary<string, int> appRoles)
        {
            var wf = new Workflow
            {
                Name = "PR_ReceivingAndReview_Workflow",
                Description = "Handles PR allocation, review, and data completeness check",
                ParentWorkflowId = parent.Id,
                Order = order,
                WorkflowStatusId = status.Id,
                CreatedBy = "System",
                CreatedAt = DateTime.UtcNow,
                Steps = new List<WorkflowStep>()
            };

            wf.Steps.AddRange(new[]
            {
                new WorkflowStep
                {
                    Name = "Allocate_PR_To_MRO",
                    Order = 1,
                    WorkflowStatusId = status.Id,
                    ActionTypeEntityId = acceptAction.Id,
                    AppRoleId = appRoles["Procurement"],
                    CreatedBy = "System",
                    CreatedAt = DateTime.UtcNow
                },
                new WorkflowStep
                {
                    Name = "Review_Service_Description",
                    Order = 2,
                    WorkflowStatusId = status.Id,
                    ActionTypeEntityId = acceptAction.Id,
                    AppRoleId = appRoles["Procurement"],
                    CreatedBy = "System",
                    CreatedAt = DateTime.UtcNow
                },
                new WorkflowStep
                {
                    Name = "Check_PR_Data_Completeness",
                    Order = 3,
                    WorkflowStatusId = status.Id,
                    ActionTypeEntityId = acceptAction.Id,
                    AppRoleId = appRoles["Procurement"],
                    CreatedBy = "System",
                    CreatedAt = DateTime.UtcNow
                },
                new WorkflowStep
                {
                    Name = "Return_PR_To_Requester",
                    Order = 4,
                    WorkflowStatusId = status.Id,
                    ActionTypeEntityId = rejectAction.Id,
                    AppRoleId = appRoles["Employee"],
                    CreatedBy = "System",
                    CreatedAt = DateTime.UtcNow
                },
                new WorkflowStep
                {
                    Name = "Wait_For_Resubmission",
                    Order = 5,
                    WorkflowStatusId = status.Id,
                    ActionTypeEntityId = acceptAction.Id,
                    AppRoleId = appRoles["Employee"],
                    CreatedBy = "System",
                    CreatedAt = DateTime.UtcNow
                },
                new WorkflowStep
                {
                    Name = "Fill_Needed_Info",
                    Order = 6,
                    WorkflowStatusId = status.Id,
                    ActionTypeEntityId = acceptAction.Id,
                    AppRoleId = appRoles["Employee"],
                    CreatedBy = "System",
                    CreatedAt = DateTime.UtcNow
                },
                new WorkflowStep
                {
                    Name = "Review_Service_Description_Again",
                    Order = 7,
                    WorkflowStatusId = status.Id,
                    ActionTypeEntityId = acceptAction.Id,
                    AppRoleId = appRoles["Procurement"],
                    isEndStep = true,
                    CreatedBy = "System",
                    CreatedAt = DateTime.UtcNow
                }
            });

            return wf;
        }

        private static Workflow CreateProviderSelectionWorkflow(Workflow parent, int order, WorkflowStatus status,
            ActionTypeEntity acceptAction, Dictionary<string, int> appRoles)
        {
            var wf = new Workflow
            {
                Name = "ServiceProvidersSelection_Workflow",
                Description = "Handles RFQ, quotation collection, and evaluations",
                ParentWorkflowId = parent.Id,
                Order = order,
                WorkflowStatusId = status.Id,
                CreatedBy = "System",
                CreatedAt = DateTime.UtcNow,
                Steps = new List<WorkflowStep>()
            };

            wf.Steps.AddRange(new[]
            {
                new WorkflowStep
                {
                    Name = "Start_Selection",
                    Order = 1,
                    WorkflowStatusId = status.Id,
                    ActionTypeEntityId = acceptAction.Id,
                    AppRoleId = appRoles["Procurement"],
                    CreatedBy = "System",
                    CreatedAt = DateTime.UtcNow
                },
                new WorkflowStep
                {
                    Name = "Collect_Quotations",
                    Order = 2,
                    WorkflowStatusId = status.Id,
                    ActionTypeEntityId = acceptAction.Id,
                    AppRoleId = appRoles["Procurement"],
                    CreatedBy = "System",
                    CreatedAt = DateTime.UtcNow
                },
                new WorkflowStep
                {
                    Name = "Technical_Evaluation",
                    Order = 3,
                    WorkflowStatusId = status.Id,
                    ActionTypeEntityId = acceptAction.Id,
                    AppRoleId = appRoles["Technical"],
                    CreatedBy = "System",
                    CreatedAt = DateTime.UtcNow
                },
                new WorkflowStep
                {
                    Name = "Financial_Assessment",
                    Order = 4,
                    WorkflowStatusId = status.Id,
                    ActionTypeEntityId = acceptAction.Id,
                    AppRoleId = appRoles["Finance"],
                    CreatedBy = "System",
                    CreatedAt = DateTime.UtcNow
                },
                new WorkflowStep
                {
                    Name = "Manager_Approval",
                    Order = 5,
                    WorkflowStatusId = status.Id,
                    ActionTypeEntityId = acceptAction.Id,
                    AppRoleId = appRoles["Manager"],
                    CreatedBy = "System",
                    CreatedAt = DateTime.UtcNow
                },
                new WorkflowStep
                {
                    Name = "Director_Approval",
                    Order = 6,
                    WorkflowStatusId = status.Id,
                    ActionTypeEntityId = acceptAction.Id,
                    AppRoleId = appRoles["Director"],
                    isEndStep = true,
                    CreatedBy = "System",
                    CreatedAt = DateTime.UtcNow
                }
            });

            return wf;
        }

        private static Workflow CreatePOIssuanceWorkflow(Workflow parent, int order, WorkflowStatus status,
            ActionTypeEntity acceptAction, Dictionary<string, int> appRoles)
        {
            var wf = new Workflow
            {
                Name = "PO_Issuance_Workflow",
                Description = "Purchase Order creation, review, and forwarding",
                ParentWorkflowId = parent.Id,
                Order = order,
                WorkflowStatusId = status.Id,
                CreatedBy = "System",
                CreatedAt = DateTime.UtcNow,
                Steps = new List<WorkflowStep>()
            };

            wf.Steps.AddRange(new[]
            {
                new WorkflowStep
                {
                    Name = "Create_PO",
                    Order = 1,
                    WorkflowStatusId = status.Id,
                    ActionTypeEntityId = acceptAction.Id,
                    AppRoleId = appRoles["Procurement"],
                    CreatedBy = "System",
                    CreatedAt = DateTime.UtcNow
                },
                new WorkflowStep
                {
                    Name = "Submit_PO_For_Approval",
                    Order = 2,
                    WorkflowStatusId = status.Id,
                    ActionTypeEntityId = acceptAction.Id,
                    AppRoleId = appRoles["Manager"],
                    CreatedBy = "System",
                    CreatedAt = DateTime.UtcNow
                },
                new WorkflowStep
                {
                    Name = "Confirm_PO_Approval",
                    Order = 3,
                    WorkflowStatusId = status.Id,
                    ActionTypeEntityId = acceptAction.Id,
                    AppRoleId = appRoles["Director"],
                    isEndStep = true,
                    CreatedBy = "System",
                    CreatedAt = DateTime.UtcNow
                }
            });

            return wf;
        }

        private static Workflow CreateInboundWorkflow(Workflow parent, int order, WorkflowStatus status,
            ActionTypeEntity acceptAction, Dictionary<string, int> appRoles)
        {
            var wf = new Workflow
            {
                Name = "InboundLogistics_Workflow",
                Description = "Inbound logistics process and material receipt",
                ParentWorkflowId = parent.Id,
                Order = order,
                WorkflowStatusId = status.Id,
                CreatedBy = "System",
                CreatedAt = DateTime.UtcNow,
                Steps = new List<WorkflowStep>()
            };

            wf.Steps.AddRange(new[]
            {
                new WorkflowStep
                {
                    Name = "Start_Inbound_Logistics",
                    Order = 1,
                    WorkflowStatusId = status.Id,
                    ActionTypeEntityId = acceptAction.Id,
                    AppRoleId = appRoles["Logistics"],
                    CreatedBy = "System",
                    CreatedAt = DateTime.UtcNow
                },
                new WorkflowStep
                {
                    Name = "Confirm_Warehouse_Receipt",
                    Order = 2,
                    WorkflowStatusId = status.Id,
                    ActionTypeEntityId = acceptAction.Id,
                    AppRoleId = appRoles["Warehouse"],
                    CreatedBy = "System",
                    CreatedAt = DateTime.UtcNow
                },
                new WorkflowStep
                {
                    Name = "Finance_Invoice_Creation",
                    Order = 3,
                    WorkflowStatusId = status.Id,
                    ActionTypeEntityId = acceptAction.Id,
                    AppRoleId = appRoles["Finance"],
                    isEndStep = true,
                    CreatedBy = "System",
                    CreatedAt = DateTime.UtcNow
                }
            });

            return wf;
        }

        private static Workflow CreatePaymentWorkflow(Workflow parent, int order, WorkflowStatus status,
            ActionTypeEntity acceptAction, Dictionary<string, int> appRoles)
        {
            var wf = new Workflow
            {
                Name = "SupplierPaymentSettlement_Workflow",
                Description = "Supplier payment settlement and finance confirmation",
                ParentWorkflowId = parent.Id,
                Order = order,
                WorkflowStatusId = status.Id,
                CreatedBy = "System",
                CreatedAt = DateTime.UtcNow,
                Steps = new List<WorkflowStep>()
            };

            wf.Steps.AddRange(new[]
            {
                new WorkflowStep
                {
                    Name = "Start_Payment_Settlement",
                    Order = 1,
                    WorkflowStatusId = status.Id,
                    ActionTypeEntityId = acceptAction.Id,
                    AppRoleId = appRoles["Finance"],
                    CreatedBy = "System",
                    CreatedAt = DateTime.UtcNow
                },
                new WorkflowStep
                {
                    Name = "Review_Payment_Documents",
                    Order = 2,
                    WorkflowStatusId = status.Id,
                    ActionTypeEntityId = acceptAction.Id,
                    AppRoleId = appRoles["Finance"],
                    CreatedBy = "System",
                    CreatedAt = DateTime.UtcNow
                },
                new WorkflowStep
                {
                    Name = "Confirm_Payment",
                    Order = 3,
                    WorkflowStatusId = status.Id,
                    ActionTypeEntityId = acceptAction.Id,
                    AppRoleId = appRoles["Finance"],
                    isEndStep = true,
                    CreatedBy = "System",
                    CreatedAt = DateTime.UtcNow
                }
            });

            return wf;
        }

        private static Workflow CreateVendorRegistrationWorkflow(Workflow parent, int order, WorkflowStatus status,
            ActionTypeEntity acceptAction, Dictionary<string, int> appRoles)
        {
            var wf = new Workflow
            {
                Name = "VendorRegistration_Workflow",
                Description = "Vendor registration workflow for new providers",
                ParentWorkflowId = parent.Id,
                Order = order,
                WorkflowStatusId = status.Id,
                CreatedBy = "System",
                CreatedAt = DateTime.UtcNow,
                Steps = new List<WorkflowStep>()
            };

            wf.Steps.AddRange(new[]
            {
                new WorkflowStep
                {
                    Name = "Start_Vendor_Registration",
                    Order = 1,
                    WorkflowStatusId = status.Id,
                    ActionTypeEntityId = acceptAction.Id,
                    AppRoleId = appRoles["Procurement"],
                    CreatedBy = "System",
                    CreatedAt = DateTime.UtcNow
                },
                new WorkflowStep
                {
                    Name = "Add_Vendor_To_System",
                    Order = 2,
                    WorkflowStatusId = status.Id,
                    ActionTypeEntityId = acceptAction.Id,
                    AppRoleId = appRoles["Procurement"],
                    CreatedBy = "System",
                    CreatedAt = DateTime.UtcNow
                },
                new WorkflowStep
                {
                    Name = "Set_Vendor_Active_Status",
                    Order = 3,
                    WorkflowStatusId = status.Id,
                    ActionTypeEntityId = acceptAction.Id,
                    AppRoleId = appRoles["Procurement"],
                    isEndStep = true,
                    CreatedBy = "System",
                    CreatedAt = DateTime.UtcNow
                }
            });

            return wf;
        }

        public static IApplicationBuilder SeedData(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var services = scope.ServiceProvider;
                SeedDataAsync(services).Wait();
            }
            return app;
        }
    }
}