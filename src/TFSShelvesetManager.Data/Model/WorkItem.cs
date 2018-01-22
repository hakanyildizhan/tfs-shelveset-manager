using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFSShelvesetManager.Data.Model
{
    public class WorkItem
    {
        public int WorkItemID { get; set; }
        public string AssignedTo { get; set; }
        public string WorkItemTitle { get; set; }
        public virtual ICollection<WorkItem> WorkItemLinks { get; set; }
        public WorkItemStatus WorkItemStatus { get; set; }
    }

    public class CodeReview : WorkItem
    {
        public CodeReviewStatus CodeReviewStatus { get; set; }
    }

    public enum WorkItemStatus
    {
        Created,
        Active,
        Completed,
        Closed
    }

    public enum CodeReviewStatus
    {
        Pending,
        PartiallyOK,
        GoodToGo
    }
}
