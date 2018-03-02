using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TFSShelvesetManager.Core.Helpers;
using TFSShelvesetManager.Core.Model;

namespace TFSShelvesetManager.Core.Service
{
    public class VersionControl : ITFSService
    {
        private VersionControlServer vcs { get; set; }

        /// <summary>
        /// <see cref="TfsTeamProjectCollection"/> associated with the TFS service
        /// </summary>
        public TfsTeamProjectCollection ProjectCollection { get; set; }

        /// <summary>
        /// Called to initialize a TFS service implementing <see cref="ITFSService"/>
        /// </summary>
        public void Initialize()
        {
            vcs = ProjectCollection.GetService<VersionControlServer>();
        }

        /// <summary>
        /// Gets a specific <see cref="Shelveset"/> belonging to a user.
        /// </summary>
        /// <param name="shelvesetName">Name of the shelveset</param>
        /// <param name="owner">Name of the owner</param>
        /// <returns></returns>
        public Shelveset GetShelveSet(string shelvesetName, string owner)
        {
            Shelveset shelveSet = vcs.QueryShelvesets(shelvesetName, owner).ToList().FirstOrDefault();
            return shelveSet;
        }

        /// <summary>
        /// Gets a specific <see cref="Shelveset"/> belonging to the current user.
        /// </summary>
        /// <param name="shelvesetName">Name of the shelveset</param>
        /// <returns></returns>
        public Shelveset GetShelveSet(string shelvesetName)
        {
            Shelveset shelveSet = vcs.QueryShelvesets(shelvesetName, vcs.AuthorizedUser).ToList().FirstOrDefault();
            return shelveSet;
        }

        /// <summary>
        /// Gets all <see cref="Shelveset"/>s belonging to the current user.
        /// </summary>
        /// <returns></returns>
        public List<Shelveset> GetShelveSets()
        {
            List<Shelveset> shelveSets = vcs.QueryShelvesets(null, vcs.AuthorizedUser).ToList();
            return shelveSets;
        }

        /// <summary>
        /// Updates a <see cref="Shelveset"/> belonging to the current user with a new one.
        /// </summary>
        /// <param name="shelvesetName">Name of the shelveset to update</param>
        /// <param name="updatedShelveSet">The new shelveset to replace the old one</param>
        public void UpdateShelveSet(string shelvesetName, Shelveset updatedShelveSet)
        {
            vcs.UpdateShelveset(shelvesetName, vcs.AuthorizedUser, updatedShelveSet);
        }

        /// <summary>
        /// Gets a <see cref="Shelveset"/>'s properties.
        /// </summary>
        /// <param name="shelvesetName">Name of the shelveset</param>
        /// <returns></returns>
        public Dictionary<string,object> GetShelvesetProperties(string shelvesetName)
        {
            Dictionary<string, object> props = new Dictionary<string, object>();
            Shelveset shelveset = this.GetShelveSet(shelvesetName);
            foreach (var property in shelveset.Properties)
            {
                props.Add(property.PropertyName, property.Value);
            }
            return props;
        }
        
        /// <summary>
        /// Gets <see cref="Workspace"/>s of the current user on the current machine by contacting the TFS server.
        /// </summary>
        /// <returns></returns>
        public List<Workspace> GetLocalWorkspaces()
        {
            return vcs.QueryWorkspaces(null, vcs.AuthorizedUser, Environment.MachineName).ToList();
        }

        /// <summary>
        /// Gets information about the <see cref="Workspace"/>s of the current user on the current machine from cache.
        /// </summary>
        /// <returns></returns>
        public List<WorkspaceInfo> GetLocalWorkspaceInfo()
        {
            Workstation.Current.EnsureUpdateWorkspaceInfoCache(vcs, vcs.AuthorizedUser);
            return Workstation.Current.GetAllLocalWorkspaceInfo().ToList();
        }

        /// <summary>
        /// Shelves pending changes found in a <see cref="Workspace"/> with a <see cref="Shelveset"/> name.
        /// </summary>
        /// <param name="shelvingArguments">Options on how to create the <see cref="Shelveset"/>.
        public void ShelvePendingChanges(ShelvingArgs shelvingArguments)
        {
            Workspace ws = GetWorkspace(shelvingArguments.WorkspaceName);
            PendingChange[] changes = ws.GetPendingChanges();

            if (changes.Length != 0)
            {
                Shelveset s = new Shelveset(vcs, shelvingArguments.ShelvesetName, ws.OwnerName);
                if (shelvingArguments.AssociatedWorkItemInfo != null)
                    s.WorkItemInfo = shelvingArguments.AssociatedWorkItemInfo;
                s.Comment = shelvingArguments.Comment;
                ws.Shelve(s, changes, shelvingArguments.ShelvingOption.ToTFSShelvingOption());
            }
        }

        /// <summary>
        /// Unshelves a <see cref="Shelveset"/> that belongs to the current user
        /// </summary>
        /// <param name="shelvingArguments">Options on how to unshelve the <see cref="Shelveset"/>
        public void Unshelve(ShelvingArgs shelvingArguments)
        {
            Workspace ws = GetWorkspace(shelvingArguments.WorkspaceName);
            ws.Unshelve(shelvingArguments.ShelvesetName, vcs.AuthorizedUser);
        }

        public void UnshelveAdvanced(ShelvingArgs shelvingArguments)
        {
            Workspace ws = GetWorkspace(shelvingArguments.WorkspaceName);
            PendingChange[] pendingChanges;
            Conflict[] conflicts;
            ws.Unshelve(shelvingArguments.ShelvesetName, vcs.AuthorizedUser, null, null, null, new string[] { "*" }, false, false, out pendingChanges, out conflicts);
        }

        private Workspace GetWorkspace(string workspaceName)
        {
            Workspace ws = GetLocalWorkspaces().Where(w => w.Name.Equals(workspaceName)).FirstOrDefault();
            if (ws == null)
                throw new Exception("Workspace not found.");
            return ws;
        }

        /// <summary>
        /// Creates a <see cref="Workspace"/>.
        /// </summary>
        /// <param name="workspaceArgs">Workspace creation arguments.</param>
        /// <returns>Created <see cref="Workspace"/>.</returns>
        public Workspace CreateWorkspace(WorkspaceArgs workspaceArgs)
        {
            CreateWorkspaceParameters parameters = new CreateWorkspaceParameters(workspaceArgs.WorkspaceName);
            parameters.Location = Microsoft.TeamFoundation.VersionControl.Common.WorkspaceLocation.Server;
            parameters.Folders = new WorkingFolder[] { new WorkingFolder(workspaceArgs.ServerItemPath, workspaceArgs.LocalPath) };
            FileHelper.CreateFolder(workspaceArgs.LocalPath);
            return vcs.CreateWorkspace(parameters);
        }
        
        /// <summary>
        /// Deletes a <see cref="Workspace"/>
        /// </summary>
        /// <param name="workspaceName">Name of the workspace to delete</param>
        public void DeleteWorkspace(string workspaceName)
        {
            Workspace ws = this.GetWorkspace(workspaceName);
            if (ws != null)
                ws.Delete();
        }
    }
}
