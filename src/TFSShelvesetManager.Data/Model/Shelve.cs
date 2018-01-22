using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFSShelvesetManager.Data.Model
{
    public class Shelve
    {
        public string ShelveName { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual ICollection<File> Files { get; set; }
        public virtual Build Build { get; set; }
        public virtual ICollection<WorkItem> LinkedWorkItems { get; set; }
    }
}
