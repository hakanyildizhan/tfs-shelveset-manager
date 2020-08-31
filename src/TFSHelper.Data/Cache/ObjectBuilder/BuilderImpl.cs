using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFSHelper.Data.Cache.ObjectBuilder
{
    public enum BuilderImpl
    {
        Build,
        CodeReview,
        File,
        PendingChanges,
        Shelve,
        WorkItem,
        Workspace
    }
}
