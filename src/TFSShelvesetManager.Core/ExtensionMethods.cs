using Microsoft.TeamFoundation.VersionControl.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFSShelvesetManager.Core.Model;

namespace TFSShelvesetManager.Core
{
    public static class TFSExtensionMethods
    {
        /// <summary>
        /// Gets the <see cref="ShelvingOptions"/> equivalent for this <see cref="ShelvingOption"/>
        /// </summary>
        /// <param name="shelvingOption"></param>
        /// <returns></returns>
        public static ShelvingOptions ToTFSShelvingOption(this ShelvingOption shelvingOption)
        {
            switch (shelvingOption)
            {
                case ShelvingOption.Normal:
                    return ShelvingOptions.None;
                case ShelvingOption.UndoPendingChanges:
                    return ShelvingOptions.Move;
                case ShelvingOption.Replace:
                    return ShelvingOptions.Replace;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
