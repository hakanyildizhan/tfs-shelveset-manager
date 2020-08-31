using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFSHelper.Data.Cache.ObjectBuilder
{
    public static class BuilderFactory
    {
        public static IObjectBuilder GetBuilder(BuilderImpl impl)
        {
            switch (impl)
            {
                case BuilderImpl.Build:
                    return new BuildBuilder();
                case BuilderImpl.CodeReview:
                    throw new NotImplementedException();
                case BuilderImpl.File:
                    return new FileBuilder();
                case BuilderImpl.PendingChanges:
                    throw new NotImplementedException();
                case BuilderImpl.Shelve:
                    return new ShelveBuilder();
                case BuilderImpl.WorkItem:
                    return new WorkItemBuilder();
                case BuilderImpl.Workspace:
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
