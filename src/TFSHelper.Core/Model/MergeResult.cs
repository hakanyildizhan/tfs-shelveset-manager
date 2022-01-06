using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFSHelper.Core.Model
{
    public class MergeResult
    {
        public int Failures { get; set; }
        public int Conflicts { get; set; }
        public long Files { get; set; }
        public int Operations { get; set; }
        public int Updates { get; set; }
        public List<string> FailureMessages { get; set; }
    }
}
