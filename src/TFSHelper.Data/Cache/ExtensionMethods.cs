using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using TFSHelper.Data.Model;

namespace TFSHelper.Data.Cache
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

        public static T CloneExcept<T, S>(this T target, S source, string[] propertyNames)
            where T : BaseModel
            where S : BaseModel
        {
            if (source == null)
            {
                return target;
            }
            Type sourceType = typeof(S);
            Type targetType = typeof(T);
            BindingFlags flags = BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance;

            PropertyInfo[] properties = sourceType.GetProperties();
            foreach (PropertyInfo sPI in properties)
            {
                if (!propertyNames.Contains(sPI.Name))
                {
                    PropertyInfo tPI = targetType.GetProperty(sPI.Name, flags);
                    if (tPI != null && tPI.PropertyType.IsAssignableFrom(sPI.PropertyType))
                    {
                        tPI.SetValue(target, sPI.GetValue(source, null), null);
                    }
                }
            }
            return target;
        }
    }
}