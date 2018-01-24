using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFSShelvesetManager.Data.Model.Base;

namespace TFSShelvesetManager.Data.Model.ObjectModel
{
    public class PendingChanges : IPendingChanges
    {
        public string Name { get; set; }
        public virtual ICollection<File> Files { get; set; }
        public virtual Workspace Workspace { get; set; }
    }
}
