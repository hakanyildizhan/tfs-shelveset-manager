using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace TFSShelvesetManager.Data.Cache
{
    public class CacheHelper
    {
        static readonly object cacheLock = new object();

        private static CacheItemPolicy cip
        {
            get
            {
                return new CacheItemPolicy()
                {
                    AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddMinutes(20))
                };
            }
        }

        #region Public Methods

        public static T GetOrInsert<T>(string cacheKey, Func<T> Action) where T : class
        {
            var cachedData = MemoryCache.Default.AddOrGetExisting(cacheKey, Action, cip) as T;
            return cachedData;
        }

        public static T Upsert<T>(string cacheKey, Func<T> Action) where T : class
        {
            Remove(cacheKey);
            var cachedData = Insert<T>(cacheKey, Action) as T;
            return cachedData;
        }

        #endregion

        private static T Get<T>(string cacheKey, Func<T> Action) where T : class
        {
            //Returns null if the string does not exist
            var cachedData = MemoryCache.Default.Get(cacheKey, null) as T;
            return cachedData;
        }
        
        private static void Remove(string cacheKey)
        {
            MemoryCache.Default.Remove(cacheKey);
        }

        private static T Insert<T>(string cacheKey, Func<T> Action) where T : class
        {
            // prevents a race condition where the cache invalidates between the contains check and the retrieval.
            lock (cacheLock)
            {
                //Check to see if anyone wrote to the cache while we where waiting our turn to write the new value.
                var cachedData = MemoryCache.Default.Get(cacheKey, null) as T;

                if (cachedData != null)
                {
                    return cachedData;
                }

                //The value still did not exist so we now write it in to the cache.
                var newData = Action();
                
                MemoryCache.Default.Set(cacheKey, newData, cip);
                return newData;
            }
        }
    }

}
