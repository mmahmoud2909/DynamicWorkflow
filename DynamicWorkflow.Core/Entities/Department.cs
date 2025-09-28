using DynamicWorkflow.Core.Entities.Users;

namespace DynamicWorkflow.Core.Entities
{
    public class Department
    {
        public Guid Id { get; set; }
        public string Name {  get; set; }
        public ICollection<ApplicationUser> Users { get; set; } = new HashSet<ApplicationUser>();
    }
}
