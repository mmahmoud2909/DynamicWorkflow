namespace DynamicWorkflow.Core.DTOs.Department
{
    public class DepartmentDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DepartmentDto(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
