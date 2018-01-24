using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFSShelvesetManager.Data.Model.Base
{
    public interface IWorkspace : IModel
    {
        string LocalPath { get; set; }
        string ServerPath { get; set; }
        string OwnerName { get; set; }
    }
}
