using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFSHelper.Data.Model;

namespace TFSHelper.Data.Cache.ObjectBuilder
{
    public class WorkItemBuilder : IObjectBuilder
    {
        public object Build<T>(T baseObject) where T : BaseModel
        {
            WorkItem workItemToCache = baseObject as WorkItem;
            return workItemToCache;
        }
    }
}
