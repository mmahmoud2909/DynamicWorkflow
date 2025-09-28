namespace DynamicWorkflow.Core.DTOs.User
{
    public class UpdateUserDto
    {
        public string? DisplayName { get; set; }
        public Guid? DepartmentId { get; set; }
        public Guid? ManagerId { get; set; }
        public bool? IsPendingDeletion { get; set; }
    }
}
