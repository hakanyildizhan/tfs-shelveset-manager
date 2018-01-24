using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFSShelvesetManager.Data.Model.Base
{
    interface IShelve : IModel
    {
        DateTime CreatedAt { get; set; }
    }
}
