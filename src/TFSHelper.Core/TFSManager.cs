using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TFSHelper.Core.Service;
using TFSHelper.Core.Credentials;

namespace TFSHelper.Core
{
    public class TFSManager : ITFSManager
    {
        private TfsTeamProjectCollection teamProjectCollection { get; set; }
        
        public TFSManager(string projectCollectionUri = "https://venus.tfs.siemens.net/tfs/TIA")
        {
            Uri tfsUri = new Uri(projectCollectionUri);
            teamProjectCollection = new TfsTeamProjectCollection(tfsUri, CredentialManager.VSSCredentials);
            teamProjectCollection.EnsureAuthenticated();
        }

        public T GetService<T>() where T : ITFSService
        {
            object type = Activator.CreateInstance(typeof(T));
            var property = type.GetType().GetProperty("ProjectCollection", BindingFlags.Public | BindingFlags.Instance);
            if (property != null && property.CanWrite)
                property.SetValue(type, teamProjectCollection, null);

            MethodInfo method = typeof(T).GetMethod("Initialize");
            if (method == null)
                throw new Exception("Method not found.");
            method.Invoke(type, null);
            return (T)type;
        }
    }
}
