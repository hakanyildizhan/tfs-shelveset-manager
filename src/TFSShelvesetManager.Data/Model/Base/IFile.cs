using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFSShelvesetManager.Data.Model.Base
{
    public interface IFile : IModel
    {
        string Name { get; set; }
        string LocalPath { get; set; }
        string ServerPath { get; set; }
        FileType FileType { get; set; }
    }

    public enum FileType
    {
        MDD,
        CS,
        Other
    }
}
