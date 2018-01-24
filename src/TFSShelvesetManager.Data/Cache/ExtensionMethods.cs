using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using TFSShelvesetManager.Data.Model;

namespace TFSShelvesetManager.Data.Cache
{
    public static class ExtensionMethods
    {
        public static string ConstructKey(this BaseModel model)
        {
			if (model is Shelve)
				return model.Identifier;
			else if (model is WorkItem)
				return model.ParentIdentifier;
			else if (model is Build)
				return model.Identifier;
			else if (model is File)
				return model.ParentIdentifier;
			else if (model is Workspace)
				return model.Identifier;
			else if (model is PendingChanges)
				return model.ParentIdentifier;
			else
				return string.Empty;
        }
    }
}