using AutoMapper;
using DynamicWorkflow.Core.DTOs.Department;
using DynamicWorkflow.Core.DTOs.User;
using DynamicWorkflow.Core.Entities;
using DynamicWorkflow.Core.Entities.Users;
using DynamicWorkflow.Core.Interfaces;
using DynamicWorkflow.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DynamicWorkflow.Services.Services
{
    public class AdminUserService : IAdminUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationIdentityDbContext _db;
        private readonly IMapper _mapper;

        public AdminUserService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ApplicationIdentityDbContext db,
            IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
            _mapper = mapper;
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            var users = await _userManager.Users
                .Include(u => u.Department)
                .Include(u => u.AppRole) // Include AppRole for better visibility
                .ToListAsync();

            var userDtos = new List<UserDto>();

            foreach (var u in users)
            {
                var role = (await _userManager.GetRolesAsync(u)).FirstOrDefault() ?? string.Empty;

                var dto = new UserDto(
                    u.Id,
                    u.UserName!,
                    u.Email!,
                    u.DisplayName,
                    u.DepartmentId,
                    u.ManagerId,
                    u.IsPendingDeletion,
                    u.RegisteredAt,
                    u.ProfilePicUrl,
                    role
                );

                userDtos.Add(dto);
            }

            return userDtos;
        }

        public async Task<UserDto?> GetUserByIdAsync(Guid id)
        {
            var u = await _userManager.Users
                .Include(x => x.Department)
                .Include(x => x.AppRole)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (u == null) return null;

            var role = (await _userManager.GetRolesAsync(u)).FirstOrDefault() ?? string.Empty;

            return new UserDto(
                u.Id,
                u.UserName!,
                u.Email!,
                u.DisplayName,
                u.DepartmentId,
                u.ManagerId,
                u.IsPendingDeletion,
                u.RegisteredAt,
                u.ProfilePicUrl,
                role
            );
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto dto)
        {
            // Validate department exists
            var dept = await _db.Departments.FindAsync(dto.DepartmentId);
            if (dept == null) throw new KeyNotFoundException("Department not found");

            var roleName = dto.Role;

            // Find or create the AppRole in AppRoles table
            var appRole = await _db.AppRoles
                .FirstOrDefaultAsync(r => r.Name.ToLower() == roleName.ToLower());

            if (appRole == null)
            {
                // Create AppRole if it doesn't exist
                appRole = new AppRole
                {
                    Name = roleName,
                    Description = $"{roleName} role"
                };
                _db.AppRoles.Add(appRole);
                await _db.SaveChangesAsync();
            }

            // Ensure Identity role exists
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var newRole = new ApplicationRole { Name = roleName };
                var roleResult = await _roleManager.CreateAsync(newRole);
                if (!roleResult.Succeeded)
                    throw new InvalidOperationException($"Failed to create Identity role '{roleName}'.");
            }

            // Create user with AppRoleId set
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = dto.Email,
                Email = dto.Email,
                DisplayName = dto.DisplayName,
                RegisteredAt = DateTime.UtcNow,
                DepartmentId = dto.DepartmentId,
                ManagerId = dto.ManagerId,
                IsPendingDeletion = false,
                AppRoleId = appRole.Id // SET THE AppRoleId HERE
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));

            // Assign Identity role for authentication/authorization
            var assignResult = await _userManager.AddToRoleAsync(user, roleName);
            if (!assignResult.Succeeded)
                throw new InvalidOperationException($"Failed to assign role '{roleName}' to user '{user.UserName}'.");

            return new UserDto(
                user.Id,
                user.UserName!,
                user.Email!,
                user.DisplayName,
                user.DepartmentId,
                user.ManagerId,
                user.IsPendingDeletion,
                user.RegisteredAt,
                user.ProfilePicUrl,
                roleName
            );
        }

        public async Task UpdateUserAsync(Guid id, UpdateUserDto dto)
        {
            var user = await _userManager.FindByIdAsync(id.ToString())
                ?? throw new KeyNotFoundException("User not found");

            if (dto.DisplayName is not null) user.DisplayName = dto.DisplayName;

            if (dto.DepartmentId.HasValue)
            {
                var dept = await _db.Departments.FindAsync(dto.DepartmentId.Value);
                if (dept == null) throw new KeyNotFoundException("Department not found");
                user.DepartmentId = dto.DepartmentId.Value;
            }

            if (dto.ManagerId.HasValue) user.ManagerId = dto.ManagerId.Value;
            if (dto.IsPendingDeletion.HasValue) user.IsPendingDeletion = dto.IsPendingDeletion.Value;

            // If role is being updated, handle both AppRoleId and Identity role
            if (!string.IsNullOrEmpty(dto.Role))
            {
                await UpdateUserRoleAsync(user, dto.Role);
            }

            var res = await _userManager.UpdateAsync(user);
            if (!res.Succeeded)
                throw new InvalidOperationException(string.Join("; ", res.Errors.Select(e => e.Description)));
        }

        private async Task UpdateUserRoleAsync(ApplicationUser user, string newRoleName)
        {
            // Find or create AppRole
            var appRole = await _db.AppRoles
                .FirstOrDefaultAsync(r => r.Name.ToLower() == newRoleName.ToLower());

            if (appRole == null)
            {
                appRole = new AppRole
                {
                    Name = newRoleName,
                    Description = $"{newRoleName} role"
                };
                _db.AppRoles.Add(appRole);
                await _db.SaveChangesAsync();
            }

            // Update AppRoleId
            user.AppRoleId = appRole.Id;

            // Ensure Identity role exists
            if (!await _roleManager.RoleExistsAsync(newRoleName))
            {
                var newRole = new ApplicationRole { Name = newRoleName };
                await _roleManager.CreateAsync(newRole);
            }

            // Remove old Identity roles and assign new one
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
            }
            await _userManager.AddToRoleAsync(user, newRoleName);
        }

        public async Task DeleteUserAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString())
                ?? throw new KeyNotFoundException("User not found");

            var res = await _userManager.DeleteAsync(user);
            if (!res.Succeeded)
                throw new InvalidOperationException(string.Join("; ", res.Errors.Select(e => e.Description)));
        }

        public async Task<List<DepartmentDto>> GetDepartmentsAsync()
        {
            return await _db.Departments
                .Select(d => new DepartmentDto(d.Id, d.Name))
                .ToListAsync();
        }

        public async Task<DepartmentDto> CreateDepartmentAsync(CreateDepartmentDto dto)
        {
            var dept = new Department { Id = Guid.NewGuid(), Name = dto.Name };
            _db.Departments.Add(dept);
            await _db.SaveChangesAsync();
            return new DepartmentDto(dept.Id, dept.Name);
        }

        public async Task<DepartmentDto?> GetDepartmentByIdAsync(Guid id)
        {
            var dept = await _db.Departments.FindAsync(id);
            if (dept == null) return null;

            return new DepartmentDto(dept.Id, dept.Name);
        }

        public async Task UpdateDepartmentAsync(Guid id, UpdateDepartmentDto dto)
        {
            var dept = await _db.Departments.FindAsync(id)
                ?? throw new KeyNotFoundException("Department not found");

            dept.Name = dto.Name;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteDepartmentAsync(Guid id)
        {
            var dept = await _db.Departments.FindAsync(id)
                ?? throw new KeyNotFoundException("Department not found");

            var hasUsers = await _db.Users.AnyAsync(u => u.DepartmentId == id);
            if (hasUsers)
                throw new InvalidOperationException("Cannot delete department with assigned users.");

            _db.Departments.Remove(dept);
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Migration utility: Fix existing users with null AppRoleId
        /// </summary>
        public async Task FixExistingUsersAppRoleIdAsync()
        {
            var usersWithNullRole = await _db.Users
                .Where(u => u.AppRoleId == null)
                .ToListAsync();

            foreach (var user in usersWithNullRole)
            {
                // Get the user's Identity roles
                var roles = await _userManager.GetRolesAsync(user);
                var primaryRole = roles.FirstOrDefault();

                if (!string.IsNullOrEmpty(primaryRole))
                {
                    // Find or create matching AppRole
                    var appRole = await _db.AppRoles
                        .FirstOrDefaultAsync(r => r.Name.ToLower() == primaryRole.ToLower());

                    if (appRole == null)
                    {
                        appRole = new AppRole
                        {
                            Name = primaryRole,
                            Description = $"{primaryRole} role"
                        };
                        _db.AppRoles.Add(appRole);
                        await _db.SaveChangesAsync();
                    }

                    user.AppRoleId = appRole.Id;
                }
                else
                {
                    // Default to Employee role if no role found
                    var defaultRole = await _db.AppRoles
                        .FirstOrDefaultAsync(r => r.Name == "Employee");

                    if (defaultRole == null)
                    {
                        defaultRole = new AppRole
                        {
                            Name = "Employee",
                            Description = "Employee role"
                        };
                        _db.AppRoles.Add(defaultRole);
                        await _db.SaveChangesAsync();
                    }

                    user.AppRoleId = defaultRole.Id;
                    await _userManager.AddToRoleAsync(user, "Employee");
                }
            }

            await _db.SaveChangesAsync();
        }
    }
}