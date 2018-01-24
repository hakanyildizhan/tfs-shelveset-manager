using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFSShelvesetManager.Data.Model
{
    public class Build : BaseModel
    {
        public string BuildDefinitionName { get; set; }
        public virtual Shelve IncludedShelve { get; set; }
        public BuildState BuildState { get; set; }
    }

	public enum BuildState
	{
		InQueue,
		Succeeded,
		Failed
	}
}
