using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using TFSHelper.Data.Cache;
using TFSHelper.Data.Cache.ObjectBuilder;
using TFSHelper.Data.Model;

namespace TFSHelper.Data.Context
{
    public class CachedContext : IShelvesetManagerContext
    {
        private static ApplicationCache applicationCache = new ApplicationCache();
        public ApplicationCache Cache { get { return applicationCache; } }

        public void Add<T>(T data) where T : BaseModel
        {
            if (data is Shelve)
            {
                IObjectBuilder builder = BuilderFactory.GetBuilder(BuilderImpl.Shelve);
                applicationCache.Insert<Shelve>(data.ConstructKey(), builder.Build<Shelve>(data as Shelve));

                builder = BuilderFactory.GetBuilder(BuilderImpl.WorkItem);
                (data as Shelve).LinkedWorkItems.ToList().ForEach(w => applicationCache.Insert<WorkItem>(w.ConstructKey(), builder.Build<WorkItem>(w as WorkItem)));

                builder = BuilderFactory.GetBuilder(BuilderImpl.File);
                (data as Shelve).Files.ToList().ForEach(f => applicationCache.Insert<File>(f.ConstructKey(), builder.Build<File>(f)));
            }
            
        }

    }
}
