using DynamicWorkflow.Core.DTOs.Department;
using DynamicWorkflow.Core.DTOs.User;

namespace DynamicWorkflow.Core.Interfaces
{
    public interface IAdminUserService
    {
        Task<List<UserDto>> GetAllUsersAsync();
        Task<UserDto?> GetUserByIdAsync(Guid id);
        Task<UserDto> CreateUserAsync(CreateUserDto dto);
        Task UpdateUserAsync(Guid id, UpdateUserDto dto);
        Task DeleteUserAsync(Guid id);

        // Departments
        Task<List<DepartmentDto>> GetDepartmentsAsync();
        Task<DepartmentDto> CreateDepartmentAsync(CreateDepartmentDto dto);
        Task UpdateDepartmentAsync(Guid id, UpdateDepartmentDto dto);
        Task DeleteDepartmentAsync(Guid id);
    }
}
