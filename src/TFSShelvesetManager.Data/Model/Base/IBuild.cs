using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFSShelvesetManager.Data.Model.Base
{
    public interface IBuild : IModel
    {
        string Name { get; set; }
        string BuildDefinitionName { get; set; }
        BuildState BuildState { get; set; }
    }

    public enum BuildState
    {
        InQueue,
        Succeeded,
        Failed
    }
}
