using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using TFSShelvesetManager.Data.Cache;

namespace TFSShelvesetManager.Data.Context
{
    public class CachedContext : IShelvesetManagerContext
    {
		private static ApplicationCache applicationCache = new ApplicationCache();

		public ApplicationCache Cache { get { return applicationCache; } }
	}
}
