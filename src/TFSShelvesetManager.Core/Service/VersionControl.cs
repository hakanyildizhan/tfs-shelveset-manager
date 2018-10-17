using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
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

        /// <summary>
        /// Performs a merge from a source branch to a target branch both of which have different MDD file versions. Workspace needs to have no pending changes prior to performing this action. Shortcomings:
        /// <para></para>
        /// Version part of imports inside target files (e.g. IMPORT "..._V15.0.0.0") as well as file headers show the old version after the operation (because files from the source branch were written so): these lines should be the same as before (as initially created by the HSP creator.)
        /// <para></para>
        /// Changes should be built to see if there is any errors. One particular thing to do is to ensure correct/new version of files are imported, in case a new, commonly used file is created. (e.g. AllVar MDD)
        /// <para></para>
        /// When the same change/changes are done on a file on both branches in different dates, merge operation may produce unchanged files showed as [merge] only. These can be undone.
        /// <para></para>
        /// If a changeset that is higher than given range exists for a target file, this changeset should not be lost - it should be merged as well. Note: to avoid this, take the starting changeset from source branch; and the ending changeset from the TARGET branch (if it is higher on considered path)
        /// <para></para>
        /// In scenarios where: a specific device was removed in HSP manifest files for a specific update, and should exist for the target service pack/update, these changes should be made manually. There is no way to automate this. Such files include: Hwcn-Sirius3RW5-Lifelist-Configuration.xml, Manifest.xml, manifest-texts-en.xml, manifest-texts-de.xml.
        /// </summary>
        /// <param name="mergeArgs">Arguments for the merge operation.</param>
        public void MergeBetweenVersions(MergeArgs mergeArgs)
        {
            this.Merge(mergeArgs);
            Workspace workspace = GetWorkspace(mergeArgs.TargetWorkspaceName);
            List<PendingChange> pendingChanges = workspace.GetPendingChangesEnumerable().ToList();
            StringBuilder fileContents = new StringBuilder();

            foreach (var change in pendingChanges)
            {
                string targetFile = Path.Combine(change.LocalOrServerFolder, change.FileName.Replace(mergeArgs.SourceMDDVersion, mergeArgs.TargetMDDVersion));

                if (change.FileName.Contains(mergeArgs.SourceMDDVersion) && 
                    vcs.ServerItemExists(change.ServerItem.Replace(mergeArgs.SourceMDDVersion, mergeArgs.TargetMDDVersion), ItemType.File))
                {
                    workspace.PendEdit(targetFile);
                    string sourceFile = Path.Combine(change.LocalOrServerFolder.Replace(mergeArgs.TargetBranch, mergeArgs.SourceBranch), change.FileName);
                    fileContents.Append(File.ReadAllText(sourceFile));
                    File.WriteAllText(targetFile, fileContents.ToString());
                    fileContents.Clear();
                    workspace.Undo(sourceFile.Replace(mergeArgs.SourceBranch, mergeArgs.TargetBranch));
                }
            }
        }

        /// <summary>
        /// Merges changes from a source branch to a target branch.
        /// </summary>
        /// <param name="mergeArgs">Arguments for the merge operation.</param>
        public void Merge(MergeArgs mergeArgs)
        {
            Workspace workspace = GetWorkspace(mergeArgs.TargetWorkspaceName);

            string sourceBranchPath = mergeArgs.SourceBranchPathToConsider;
            string targetBranchPath = mergeArgs.TargetBranchPathToConsider;
            VersionSpec changesetStart = new ChangesetVersionSpec(mergeArgs.ChangesetStart);
            VersionSpec changesetEnd = new ChangesetVersionSpec(mergeArgs.ChangesetEnd);

            MergeOptions mergeOption = MergeOptions.None;
            if (mergeArgs.Baseless) mergeOption = MergeOptions.Baseless;

            // pend the merge
            workspace.Merge(sourceBranchPath, targetBranchPath, changesetStart, changesetEnd, LockLevel.CheckOut, RecursionType.Full, mergeOption);
        }
    }
}
