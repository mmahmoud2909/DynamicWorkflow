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
        private readonly ApplicationIdentityDbContext _db;
        private readonly IMapper _mapper; // optional AutoMapper

        public AdminUserService(UserManager<ApplicationUser> userManager, ApplicationIdentityDbContext db, IMapper mapper)
        {
            _userManager = userManager;
            _db = db;
            _mapper = mapper;
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            var users = await _userManager.Users
                .Include(u => u.Department)
                .ToListAsync();

            return users.Select(u => new UserDto(
                u.Id,
                u.UserName!,
                u.Email!,
                u.DisplayName,
                u.DepartmentId,
                u.ManagerId,
                u.IsPendingDeletion,
                u.RegisteredAt,
                u.ProfilePicUrl
            )).ToList();
        }

        public async Task<UserDto?> GetUserByIdAsync(Guid id)
        {
            var u = await _userManager.Users.Include(x => x.Department).FirstOrDefaultAsync(x => x.Id == id);
            if (u == null) return null;
            return new UserDto(u.Id, u.UserName!, u.Email!, u.DisplayName, u.DepartmentId, u.ManagerId, u.IsPendingDeletion, u.RegisteredAt, u.ProfilePicUrl);
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto dto)
        {
            // validate department exists
            var dept = await _db.Departments.FindAsync(dto.DepartmentId);
            if (dept == null) throw new KeyNotFoundException("Department not found");

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = dto.Email,
                Email = dto.Email,
                DisplayName = dto.DisplayName,
                RegisteredAt = DateTime.UtcNow,
                DepartmentId = dto.DepartmentId,
                ManagerId = dto.ManagerId,
                IsPendingDeletion = false
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));

            return new UserDto(user.Id, user.UserName!, user.Email!, user.DisplayName, user.DepartmentId, user.ManagerId, user.IsPendingDeletion, user.RegisteredAt, user.ProfilePicUrl);
        }

        public async Task UpdateUserAsync(Guid id, UpdateUserDto dto)
        {
            var user = await _userManager.FindByIdAsync(id.ToString()) ?? throw new KeyNotFoundException("User not found");

            if (dto.DisplayName is not null) user.DisplayName = dto.DisplayName;
            if (dto.DepartmentId.HasValue)
            {
                var dept = await _db.Departments.FindAsync(dto.DepartmentId.Value);
                if (dept == null) throw new KeyNotFoundException("Department not found");
                user.DepartmentId = dto.DepartmentId.Value;
            }
            if (dto.ManagerId.HasValue) user.ManagerId = dto.ManagerId.Value;
            if (dto.IsPendingDeletion.HasValue) user.IsPendingDeletion = dto.IsPendingDeletion.Value;

            var res = await _userManager.UpdateAsync(user);
            if (!res.Succeeded) throw new InvalidOperationException(string.Join("; ", res.Errors.Select(e => e.Description)));
        }

        public async Task DeleteUserAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString()) ?? throw new KeyNotFoundException("User not found");
            var res = await _userManager.DeleteAsync(user);
            if (!res.Succeeded) throw new InvalidOperationException(string.Join("; ", res.Errors.Select(e => e.Description)));
        }

        // Departments CRUD
        public async Task<List<DepartmentDto>> GetDepartmentsAsync()
        {
            return await _db.Departments.Select(d => new DepartmentDto(d.Id, d.Name)).ToListAsync();
        }

        public async Task<DepartmentDto> CreateDepartmentAsync(CreateDepartmentDto dto)
        {
            var dept = new Department { Id = Guid.NewGuid(), Name = dto.Name };
            _db.Departments.Add(dept);
            await _db.SaveChangesAsync();
            return new DepartmentDto(dept.Id, dept.Name);
        }

        public async Task UpdateDepartmentAsync(Guid id, UpdateDepartmentDto dto)
        {
            var dept = await _db.Departments.FindAsync(id) ?? throw new KeyNotFoundException("Department not found");
            dept.Name = dto.Name;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteDepartmentAsync(Guid id)
        {
            var dept = await _db.Departments.FindAsync(id) ?? throw new KeyNotFoundException("Department not found");

            // Optional: ensure no users belong to it, or set their DepartmentId to null if nullable.
            var hasUsers = await _db.Users.AnyAsync(u => u.DepartmentId == id);
            if (hasUsers) throw new InvalidOperationException("Cannot delete department with assigned users.");

            _db.Departments.Remove(dept);
            await _db.SaveChangesAsync();
        }
    }
}
