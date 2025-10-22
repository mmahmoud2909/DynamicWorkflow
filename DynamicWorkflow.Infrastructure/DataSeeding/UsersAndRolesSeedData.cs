using DynamicWorkflow.Core.Entities;
using DynamicWorkflow.Core.Entities.Users;
using DynamicWorkflow.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DynamicWorkflow.Infrastructure.DataSeeding
{
    public class UsersAndRolesSeedData
    {
            public static async Task SeedAsync(IServiceProvider services)
            {
                using var scope = services.CreateScope();//we create scope of service i need and already registed in Program 
                var context = scope.ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

               // await context.Database.MigrateAsync();

                // 1️⃣ Roles
                string[] roleNames = { "Admin", "Manager", "Employee", "Procurement", "Finance", "Technical", "Warehouse", "Logistics", "HR", "Planning" };
                foreach (var roleName in roleNames)
                {
                    if (!await roleManager.RoleExistsAsync(roleName))
                        await roleManager.CreateAsync(new ApplicationRole { Name = roleName });
                }

                // 2️⃣ Departments
                string[] departments = { "IT", "Finance", "Procurement", "Technical", "Warehouse", "Logistics", "HR", "Planning" };
                foreach (var deptName in departments)
                {
                    if (!await context.Departments.AnyAsync(d => d.Name == deptName))
                        context.Departments.Add(new Department { Id = Guid.NewGuid(), Name = deptName });
                }
                await context.SaveChangesAsync();

                // 3️⃣ Department-based users
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
                            DisplayName = u.Display,
                            RegisteredAt = DateTime.UtcNow,
                            DepartmentId = dept.Id
                        };
                        var result = await userManager.CreateAsync(user, "Pass@123");
                        if (result.Succeeded)
                            await userManager.AddToRoleAsync(user, u.Role);
                    }
                }
            }
        }
    }
