using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using TFSShelvesetManager.Data.Model;
using System.Collections.ObjectModel;

namespace TFSShelvesetManager.Data.Cache
{
    internal class PolicyFactory
    {
        static DateTimeOffset Expiration = new DateTimeOffset(DateTime.Now.AddMinutes(20));
        internal static CacheItemPolicy GetPolicy<T>() where T : BaseModel
        {
            return ConstructPolicy<T>();
        }

        private static CacheItemPolicy ConstructPolicy<T>() where T : BaseModel
        {
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = Expiration;
            policy.ChangeMonitors.Add(new DataChangeMonitor(nameof(T)));
            return policy;
        }

    }
}
