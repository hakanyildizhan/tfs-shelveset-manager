using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFSShelvesetManager.Data.Model.Base;

namespace TFSShelvesetManager.Data.Model.ObjectModel
{
    public class CodeReview : ICodeReview
    {
        public string Name { get; set; }
        public int CodeReviewID { get; set; }
        public string AssignedTo { get; set; }
        public CodeReviewStatus CodeReviewStatus { get; set; }
    }
}
