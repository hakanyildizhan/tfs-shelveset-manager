using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MergeHelper
{
    public class FileComparison
    {
        public string SourceFilePath { get; set; }
        public string TargetFilePath { get; set; }
        public ComparisonResult ComparisonResult { get; set; }
    }
}
