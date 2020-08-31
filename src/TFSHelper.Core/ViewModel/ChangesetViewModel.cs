using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFSHelper.Core.ViewModel
{
    public class ChangesetViewModel
    {
        public int ID { get; set; }
        public string Owner { get; set; }
        public string Comment { get; set; }
        public List<WorkitemViewModel> AssociatedWorkitems { get; set; }
    }
}
