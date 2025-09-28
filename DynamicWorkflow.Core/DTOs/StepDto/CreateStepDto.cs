using DynamicWorkflow.Core.Entities.Users;
using DynamicWorkflow.Core.Enums;

namespace DynamicWorkflow.Core.DTOs.StepDto
{
    public class CreateStepDto
    {
        public string stepName {  get; set; }
        public Roles assignedRole { get; set; }
        public bool isEndStep { get; set; }
        public ActionType stepActionTypes { get; set; } 
        public string? comments { get; set; }

    }
}
