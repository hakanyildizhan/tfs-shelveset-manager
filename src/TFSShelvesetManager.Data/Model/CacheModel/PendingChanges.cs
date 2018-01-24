using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFSShelvesetManager.Data.Model.Base;

namespace TFSShelvesetManager.Data.Model.CacheModel
{
    public class PendingChanges : IPendingChanges
    {
        public string Name { get; set; }
        public ICollection<string> File_Names { get; set; }
        public string Workspace_Name { get; set; }
    }
}
