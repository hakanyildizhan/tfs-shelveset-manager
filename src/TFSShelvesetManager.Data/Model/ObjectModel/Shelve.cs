using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFSShelvesetManager.Data.Model.Base;

namespace TFSShelvesetManager.Data.Model.ObjectModel
{
    public class Shelve : IShelve
    {
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual ICollection<File> Files { get; set; }
        public virtual Build Build { get; set; }
        public virtual ICollection<WorkItem> LinkedWorkItems { get; set; }
    }
}
