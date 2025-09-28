using DynamicWorkflow.Core.Entities.Users;
using DynamicWorkflow.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWorkflow.Core.DTOs.StepDto
{
    public class UpdateStepDto
    {
        public string? stepName { get; set; }
        public string? comments { get; set; }
        public Roles? assignedRole { get; set; }
        public bool? isEndStep { get; set; }
        public ActionType? stepActionTypes { get; set; }
    }
}
