using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFSShelvesetManager.Data.Model.Base
{
    public interface IWorkItem : IModel
    {
        int WorkItemID { get; set; }
        string AssignedTo { get; set; }
        WorkItemStatus WorkItemStatus { get; set; }
    }
    
    public enum WorkItemStatus
    {
        Created,
        Active,
        Completed,
        Closed
    }
    
}
