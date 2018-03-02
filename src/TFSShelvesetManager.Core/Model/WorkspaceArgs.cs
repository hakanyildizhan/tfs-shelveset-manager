using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFSShelvesetManager.Core.Model
{
    public class WorkspaceArgs
    {
        public string ServerItemPath { get; set; }
        public string LocalPath { get; set; }
        public string WorkspaceName { get; set; }
    }
}
