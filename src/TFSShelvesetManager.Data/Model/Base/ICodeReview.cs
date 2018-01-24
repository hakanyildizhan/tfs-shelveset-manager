using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFSShelvesetManager.Data.Model.Base
{
    public interface ICodeReview : IModel
    {
        int CodeReviewID { get; set; }
        string AssignedTo { get; set; }
        CodeReviewStatus CodeReviewStatus { get; set; }
    }

    public enum CodeReviewStatus
    {
        Pending,
        PartiallyOK,
        GoodToGo
    }
}
