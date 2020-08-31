using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFSHelper.Data.Context
{
    public static class ContextFactory
    {
        static Lazy<List<IShelvesetManagerContext>> contextList;
        static ContextFactory()
        {
            contextList = new Lazy<List<IShelvesetManagerContext>>();
            contextList.Value.Add(new CachedContext());
            contextList.Value.Add(new SqlContext());
        }
        public static IShelvesetManagerContext GetContext<T>(ContextImpl contextImplementation)
        {
            switch (contextImplementation)
            {
                case ContextImpl.Cache:
                    return contextList.Value.Where(c => c is CachedContext).First();
                case ContextImpl.Sql:
                    return contextList.Value.Where(c => c is SqlContext).First();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
