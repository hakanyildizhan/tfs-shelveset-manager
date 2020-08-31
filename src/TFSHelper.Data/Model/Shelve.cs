using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFSHelper.Data.Model
{
    public class Shelve : BaseModel
    {
        public DateTime CreatedAt { get; set; }
        public virtual ICollection<File> Files { get; set; }
        public virtual Build Build { get; set; }
        public virtual ICollection<WorkItem> LinkedWorkItems { get; set; }
    }
}
