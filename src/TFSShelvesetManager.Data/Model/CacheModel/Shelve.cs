using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFSShelvesetManager.Data.Model.Base;

namespace TFSShelvesetManager.Data.Model.CacheModel
{
    public class Shelve : IShelve
    {
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<string> File_Names { get; set; }
        public string Build_Name { get; set; }
        public ICollection<string> LinkedWorkItem_Names { get; set; }
    }
}
