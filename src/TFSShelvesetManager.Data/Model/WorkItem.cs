using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFSShelvesetManager.Data.Model
{
    public class WorkItem : BaseModel
    {
        public string AssignedTo { get; set; }
        public virtual ICollection<WorkItem> WorkItemLinks { get; set; }
        public WorkItemStatus WorkItemStatus { get; set; }
		public WorkItemType WorkItemType { get; set; }
	}

	public enum WorkItemType
	{
		RQ,
		Task
	}

	public enum WorkItemStatus
	{
		Created,
		Active,
		Completed,
		Closed
	}
}
