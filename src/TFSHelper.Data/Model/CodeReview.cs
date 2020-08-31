using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFSHelper.Data.Model
{
    public class CodeReview : WorkItem
    {
        public virtual ICollection<CodeReview> CodeReviewLinks { get; set; }
        public CodeReviewStatus CodeReviewStatus { get; set; }
    }

	public enum CodeReviewStatus
	{
		Pending,
		PartiallyOK,
		GoodToGo
	}
}
