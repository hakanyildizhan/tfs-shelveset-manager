using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFSHelper.Data.Model
{
    public abstract class BaseModel
    {
        public string Identifier { get; set; }
		public string ParentIdentifier { get; set; }
	}
}
