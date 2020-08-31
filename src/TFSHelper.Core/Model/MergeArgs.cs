using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFSHelper.Core.Model
{
    public class MergeArgs
    {
        public string TargetWorkspacePath { get; set; }
        public string TargetWorkspaceName { get; set; }
        public string SourceBranch { get; set; }
        public string TargetBranch { get; set; }
        public string SourceBranchPathToConsider { get; set; }
        public string TargetBranchPathToConsider { get; set; }
        public int ChangesetStart { get; set; }
        public int ChangesetEnd { get; set; }
        public bool Baseless { get; set; } = false;
        public string SourceMDDVersion { get; set; }
        public string TargetMDDVersion { get; set; }
    }
}
