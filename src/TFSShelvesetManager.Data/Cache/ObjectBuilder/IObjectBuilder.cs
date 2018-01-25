using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFSShelvesetManager.Data.Model;

namespace TFSShelvesetManager.Data.Cache.ObjectBuilder
{
    public interface IObjectBuilder
    {
        object Build<T>(T baseObject) where T : BaseModel;
    }
}
