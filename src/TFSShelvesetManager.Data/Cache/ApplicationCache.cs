using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using TFSShelvesetManager.Data.Model;
using TFSShelvesetManager.Data.Model.Base;

namespace TFSShelvesetManager.Data.Cache
{
    public class ApplicationCache : MemoryCache
    {

        public ApplicationCache() : base("ApplicationCache") { }
        public ApplicationCache(string name, NameValueCollection config = null) : base(name, config)
        {
        }
        public override DefaultCacheCapabilities DefaultCacheCapabilities
        {
            get { return DefaultCacheCapabilities.CacheRegions; }
        }

        public void Purge()
        {
            DataChangeMonitor.Signal();
        }

        public void Purge<T>() where T : IModel
        {
            DataChangeMonitor.Signal(nameof(T));
        }

        public void Insert<T>(string key, T value, string region) where T : class, IModel
        {
            Set(key, value, GetPolicyForModel<T>(), region);
        }

        public T Get<T>(string key, string region) where T : class, IModel
        {
            return Get(key, region) as T;
        }

        #region Base Methods
        public override void Set(string key, object value, CacheItemPolicy policy, string regionName = null)
        {
            base.Set(key, value, policy, regionName);
        }

        public override object Get(string key, string regionName = null)
        {
            return base.Get(key, regionName);
        }

        public override object Remove(string key, string regionName = null)
        {
            return base.Remove(key, regionName);
        }
        #endregion

        private CacheItemPolicy GetPolicyForModel<T>() where T : IModel
        {
            return PolicyFactory.GetPolicy<T>();
        }
    }
}
