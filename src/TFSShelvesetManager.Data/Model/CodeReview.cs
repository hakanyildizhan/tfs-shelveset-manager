using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFSShelvesetManager.Data.Model
{
    public class CodeReview : WorkItem
    {
        public CodeReviewStatus CodeReviewStatus { get; set; }
    }

	public enum CodeReviewStatus
	{
		Pending,
		PartiallyOK,
		GoodToGo
	}
}
