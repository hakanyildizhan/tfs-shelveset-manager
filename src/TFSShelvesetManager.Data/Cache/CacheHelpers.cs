using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using TFSShelvesetManager.Data.Model;
using TFSShelvesetManager.Data.Model.Base;

namespace TFSShelvesetManager.Data.Cache
{
    public static class CacheHelpers
    {
        public static string ConstructKey(this IModel model)
        {
            return string.IsNullOrEmpty(model.Name) ? $"{nameof(model)}" : $"{nameof(model)}_{model.Name}";
        }
    }

}
