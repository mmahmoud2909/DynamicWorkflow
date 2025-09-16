using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWorkflow.Core.Enums
{
    public enum ActionType
    {
        Create,
        Accept,
        Reject, 
        Hold,
        Notify,
        Skip,
        RequestChanges
    }
}
