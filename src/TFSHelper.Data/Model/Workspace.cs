using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFSHelper.Data.Model
{
    public class Workspace : BaseModel
    {
        public string LocalPath { get; set; }
        public string ServerPath { get; set; }
        public string OwnerName { get; set; }
        public virtual PendingChanges PendingChanges { get; set; }
    }
}
