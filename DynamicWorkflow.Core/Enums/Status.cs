using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWorkflow.Core.Enums
{
    public enum Status
    {
        Created,
        Approved,
        Accepted,
        Rejected,
        ONHold,
        Pending,
        Completed,
        Skipped,
        InProgress,
        Terminated
    }
}
