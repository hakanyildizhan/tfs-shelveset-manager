﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFSShelvesetManager.Data.Model;

namespace TFSShelvesetManager.Data.Cache.ObjectBuilder
{
    public class BuildBuilder : IObjectBuilder
    {
        public object Build<T>(T baseObject) where T : BaseModel
        {
            Build buildToCache = baseObject as Build;
            return buildToCache;
        }
    }
}
