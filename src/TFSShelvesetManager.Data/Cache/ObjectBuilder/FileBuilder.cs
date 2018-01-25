using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFSShelvesetManager.Data.Model;

namespace TFSShelvesetManager.Data.Cache.ObjectBuilder
{
    public class FileBuilder : IObjectBuilder
    {
        public object Build<T>(T baseObject) where T : BaseModel
        {
            File fileToCache = baseObject as File;
            return fileToCache;
        }
    }
}
