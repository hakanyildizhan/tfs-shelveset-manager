using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFSHelper.Core.ViewModel;

namespace MergeHelper
{
    public class FileChangeSummary
    {
        public string ConvertedFilePath { get; set; }
        public string SourceFilePath { get; set; }
        public string TargetFilePath { get; set; }
        public ComparisonResult ComparisonResult { get; set; }
        public List<ChangesetViewModel> Changesets { get; set; }
    }
}
