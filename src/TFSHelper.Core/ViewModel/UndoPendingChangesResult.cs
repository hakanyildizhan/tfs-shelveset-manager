using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFSHelper.Core.ViewModel
{
    public enum UndoPendingChangesResult
    {
        Success,
        PartialSuccess,
        Failure,
        NoAction
    }
}
