using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFSHelper.Core.Service;

namespace TFSHelper.Core
{
    public interface ITFSManager
    {
        T GetService<T>() where T : ITFSService;
    }
}
