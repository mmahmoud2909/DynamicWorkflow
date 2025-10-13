using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWorkflow.Core.Enums
{
    public enum Status
    {
        Created,//0
        Approved,//1
        Accepted,//2
        Rejected,//3
        ONHold,//4
        Pending,//5
        Completed,//6
        Skipped,//7
        InProgress,//8
        Terminated//9
    }
}
