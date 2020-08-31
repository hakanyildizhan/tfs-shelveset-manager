using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFSHelper.Data.Model;

namespace TFSHelper.Data.Cache.ObjectBuilder
{
    public class ShelveBuilder : IObjectBuilder
    {
        public object Build<T>(T baseObject) where T : BaseModel
        {
            var skipProperties = new[] { "Files", "Build", "LinkedWorkItems" };
            Shelve shelveToCache = new Shelve().CloneExcept(baseObject as Shelve, skipProperties);
            return shelveToCache;
        }
    }
}
