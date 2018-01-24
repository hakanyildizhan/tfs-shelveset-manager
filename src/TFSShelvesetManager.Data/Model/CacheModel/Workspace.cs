using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFSShelvesetManager.Data.Model.Base;

namespace TFSShelvesetManager.Data.Model.CacheModel
{
    public class Workspace : IWorkspace
    {
        public string Name { get; set; }
        public string LocalPath { get; set; }
        public string ServerPath { get; set; }
        public string OwnerName { get; set; }
        public string PendingChanges_Name { get; set; }
    }
}
