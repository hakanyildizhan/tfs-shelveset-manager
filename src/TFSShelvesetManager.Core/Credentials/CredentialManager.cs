using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Microsoft.VisualStudio.Services.Common;

namespace TFSShelvesetManager.Core.Credentials
{
    internal class CredentialManager
    {
        private static ICredentials _userCredentials;
        public static ICredentials UserCredentials
        {
            get { return _userCredentials ?? (_userCredentials = CredentialCache.DefaultCredentials); }
        }

        public static VssCredentials VSSCredentials
        {
            get { return new VssCredentials(new WindowsCredential(UserCredentials)); }
        }
    }
}
