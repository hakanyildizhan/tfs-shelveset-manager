using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFSShelvesetManager.Data.Model
{
    public class File : BaseModel
    {
        public string LocalPath { get; set; }
        public string ServerPath { get; set; }
        public FileType FileType { get; set; }
    }

	public enum FileType
	{
		MDD,
		CS,
		Other
	}
}
