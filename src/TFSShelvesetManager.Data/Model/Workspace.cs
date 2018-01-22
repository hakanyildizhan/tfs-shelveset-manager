using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFSShelvesetManager.Data.Model
{
    public class Workspace
    {
        public string WorkspaceName { get; set; }
        public string LocalPath { get; set; }
        public string ServerPath { get; set; }
        public string OwnerName { get; set; }
        public virtual PendingChanges PendingChanges { get; set; }
    }
}
