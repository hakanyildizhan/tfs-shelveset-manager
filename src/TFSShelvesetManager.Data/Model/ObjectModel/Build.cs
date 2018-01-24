using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFSShelvesetManager.Data.Model.Base;

namespace TFSShelvesetManager.Data.Model.ObjectModel
{
    public class Build : IBuild
    {
        public string Name { get; set; }
        public string BuildDefinitionName { get; set; }
        public virtual Shelve IncludedShelve { get; set; }
        public BuildState BuildState { get; set; }
    }
}
