using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFSShelvesetManager.Data.Model
{
    public class PendingChanges : BaseModel
    {
        public virtual ICollection<File> Files { get; set; }
        public virtual Workspace Workspace { get; set; }
    }
}
