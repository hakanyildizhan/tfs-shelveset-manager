using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using TFSShelvesetManager.Data.Model;

namespace TFSShelvesetManager.Data.Cache
{
    public class ApplicationCache : MemoryCache
    {

		#region Constructor
		public ApplicationCache() : base("ApplicationCache") { }
		public ApplicationCache(string name, NameValueCollection config = null) : base(name, config)
		{
		} 
		#endregion

		public override DefaultCacheCapabilities DefaultCacheCapabilities
        {
            get { return DefaultCacheCapabilities.CacheRegions; }
        }

		#region Public Methods
		public void Insert<T>(string key, T value) where T : BaseModel
		{
			Set(key, value, GetPolicyForModel<T>(), nameof(T));
		}

		public T Get<T>(string key) where T : BaseModel
		{
			return Get(key, nameof(T)) as T;
		}

		public void Purge()
		{
			DataChangeMonitor.Signal();
		}

		public void Purge<T>() where T : BaseModel
		{
			DataChangeMonitor.Signal(nameof(T));
		} 
		#endregion

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

		#region Helpers
		private CacheItemPolicy GetPolicyForModel<T>() where T : BaseModel
		{
			return PolicyFactory.GetPolicy<T>();
		} 
		#endregion
	}
}
