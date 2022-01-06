using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TFSHelper.Core.Helpers;
using TFSHelper.Core.Model;
using TFSHelper.Core.ViewModel;

namespace TFSHelper.Core.Service
{
    public class VersionControl : IVersionControl
    {
        private VersionControlServer vcs { get; set; }
        private WorkItemStore ws { get; set; }

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
            ws = ProjectCollection.GetService<WorkItemStore>();
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
        /// Undoes pending changes on a workspace. Returns true if the operation was successful.
        /// </summary>
        /// <param name="localPath"></param>
        public UndoPendingChangesResult UndoPendingChanges(string localPath)
        {
            UndoPendingChangesResult result = UndoPendingChangesResult.Failure;
            Workspace ws = vcs.GetWorkspace(localPath);
            if (ws != null)
            {
                PendingChange[] pendingChanges = ws.GetPendingChanges();
                if (!pendingChanges.Any())
                    result = UndoPendingChangesResult.NoAction;
                else
                {
                    int changeCount = pendingChanges.Count();
                    int undoneCount = ws.Undo(pendingChanges);
                    if (changeCount == undoneCount)
                        result = UndoPendingChangesResult.Success;
                    else if (changeCount != undoneCount && changeCount > 0)
                        result = UndoPendingChangesResult.PartialSuccess;
                }
            }
            return result;
        }

        /// <summary>
        /// Gets full paths of files that exist under given source-controlled directory, but not yet added to source control themselves. 
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public List<string> GetLocalItemsNotUnderSourceControl(string directory)
        {
            List<string> fileList = new List<string>();
            Workspace ws = vcs.GetWorkspace(directory);
            if (ws != null)
            {
                List<WorkspaceDifference> differences = ws.Diff(new ItemSpec[] { new ItemSpec(directory, RecursionType.Full) }).ToList();
                foreach (WorkspaceDifference difference in differences)
                    fileList.Add(difference.LocalItem);
            }
            return fileList;
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
        /// Gets the latest version of files found on the provided path from TFS.
        /// </summary>
        /// <param name="path">Local path of a directory or file.</param>
        public void GetLatest(string path)
        {
            Workspace ws = vcs.GetWorkspace(path);
            if (ws != null)
            {
                string serverPath = ws.GetServerItemForLocalItem(path);
                string[] items = new string[1] { serverPath };
                ws.Get(items, VersionSpec.Latest, RecursionType.Full, GetOptions.Overwrite);
            }
        }

        /// <summary>
        /// Gets all changesets committed on the specified file, optionally excluding changes that occurred before given changeset number.
        /// </summary>
        /// <param name="localFilePath"></param>
        /// <param name="startingChangesetNumber"></param>
        /// <param name="workitemTypeFilter"></param>
        /// <returns></returns>
        public List<ChangesetViewModel> GetChangesetsForFile(string localFilePath, int startingChangesetNumber = 0, string[] workitemTypeFilter = null)
        {
            if (string.IsNullOrEmpty(localFilePath) || !FileHelper.FileExists(localFilePath))
                return null;
            else
            {
                Workspace ws = vcs.GetWorkspace(localFilePath);
                if (ws != null)
                {
                    List<ChangesetViewModel> changesetList = new List<ChangesetViewModel>();
                    List<Changeset> changesets = vcs.QueryHistory(localFilePath, RecursionType.Full).Where(c => c.ChangesetId > startingChangesetNumber).ToList();
                    foreach (Changeset changeset in changesets)
                    {
                        ChangesetViewModel cs = new ChangesetViewModel()
                        {
                            ID = changeset.ChangesetId,
                            Owner = changeset.OwnerDisplayName,
                            Comment = changeset.Comment,
                            AssociatedWorkitems = new List<WorkitemViewModel>()
                        };

                        List<WorkItem> workitems = new List<WorkItem>();

                        if (workitemTypeFilter != null && changeset.WorkItems.Select(w => w.Type.Name).Intersect(workitemTypeFilter).Any()) // There is at least one WI that matches the filter
                        {
                            workitems = changeset.WorkItems.ToList().Where(w => workitemTypeFilter.Intersect(new string[] { w.Type.Name }).Any()).ToList();
                        }
                        else if (workitemTypeFilter != null && changeset.WorkItems.Any()) // there are only workitems that don't match the filter - look for parents of tasks
                        {
                            List<WorkItem> tasks = changeset.WorkItems.Where(w => w.Type.Name.Equals("Task")).ToList();
                            if (tasks.Any())
                            {
                                List<WorkItem> parents = new List<WorkItem>();
                                tasks.ForEach(t => parents.AddRange(GetParentWorkItems(t.Id)));
                                workitems = parents.Where(w => workitemTypeFilter.Intersect(new string[] { w.Type.Name }).Any()).ToList();
                            }
                        }
                        else if (workitemTypeFilter == null)
                        {
                            workitems = changeset.WorkItems.ToList();
                        }

                        workitems.ForEach(w =>
                                cs.AssociatedWorkitems.Add(
                                    new WorkitemViewModel()
                                    {
                                        ID = w.Id,
                                        Type = w.Type.Name,
                                        Title = w.Title
                                    }));

                        changesetList.Add(cs);
                    }
                    return changesetList;
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// Returns parent work items for given work item ID.
        /// </summary>
        /// <param name="workitemID"></param>
        /// <returns></returns>
        private List<WorkItem> GetParentWorkItems(int workitemID)
        {
            WorkItem workItem = ws.GetWorkItem(workitemID);
            if (workItem != null)
            {
                List<WorkItemLink> parents = workItem.WorkItemLinks.Cast<WorkItemLink>().Where(x => x.LinkTypeEnd.Name == "Parent").ToList();
                if (parents != null && parents.Any())
                {
                    List<WorkItem> workItems = new List<WorkItem>();
                    parents.Select(p => p.TargetId).ToList().ForEach(w => workItems.Add(ws.GetWorkItem(w)));
                    return workItems;
                }
                else
                    return null;
            }
            else
                return null;
        }

        /// <summary>
        /// Checks out given files.
        /// </summary>
        /// <param name="fileList"></param>
        public void CheckoutFiles(List<string> fileList)
        {
            foreach (string filePath in fileList)
            {
                if (FileHelper.FileExists(filePath))
                {
                    Workspace ws = vcs.GetWorkspace(filePath);
                    if (ws != null)
                    {
                        ws.PendEdit(filePath);
                    }
                }
            }
        }

        /// <summary>
        /// Adds a list of files to the source control. Same workspace should contain all items.
        /// </summary>
        /// <param name="fileList"></param>
        public void AddFilesToSourceControl(List<string> fileList)
        {
            bool parentDirectoryAlreadyAdded = false;
            string commonFolder = string.Empty;

            foreach (string filePath in fileList)
            {
                string parentDirectoryOfCurrentFile = FileHelper.GetParentDirectory(filePath);
                Workspace ws = vcs.GetWorkspace(parentDirectoryOfCurrentFile);
                if (ws != null)
                {
                    // check if all other files are contained in this folder
                    foreach (string filePathInner in fileList)
                    {
                        if (!filePathInner.Contains(parentDirectoryOfCurrentFile))
                            break;
                        commonFolder = parentDirectoryOfCurrentFile; // a common folder is found
                    }
                }
                if (!string.IsNullOrEmpty(commonFolder))
                {
                    ws = vcs.GetWorkspace(commonFolder);
                    if (ws != null)
                    {
                        ws.PendAdd(fileList.ToArray());
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the ID of the latest changeset associated with given path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public int GetLastChangesetIDForPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return 0;
            else
            {
                Workspace ws = vcs.GetWorkspace(path);
                if (ws != null)
                    return vcs.QueryHistory(path, RecursionType.Full).OrderByDescending(c => c.ChangesetId).FirstOrDefault().ChangesetId;
                else
                    return 0;
            }
        }

        /// <summary>
        /// Gets changeset IDs associated with given path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public List<int> GetChangesetsForPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;
            else
            {
                Workspace ws = vcs.GetWorkspace(path);
                if (ws != null)
                    return vcs.QueryHistory(path, RecursionType.Full).Select(c => c.ChangesetId).ToList();
                else
                    return null;
            }
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
                string targetFile = FileHelper.CombinePaths(change.LocalOrServerFolder, change.FileName.Replace(mergeArgs.SourceMDDVersion, mergeArgs.TargetMDDVersion));

                if (change.FileName.Contains(mergeArgs.SourceMDDVersion) && 
                    vcs.ServerItemExists(change.ServerItem.Replace(mergeArgs.SourceMDDVersion, mergeArgs.TargetMDDVersion), ItemType.File))
                {
                    workspace.PendEdit(targetFile);
                    string sourceFile = FileHelper.CombinePaths(change.LocalOrServerFolder.Replace(mergeArgs.TargetBranch, mergeArgs.SourceBranch), change.FileName);
                    fileContents.Append(FileHelper.ReadFile(sourceFile));
                    FileHelper.WriteToFile(targetFile, fileContents.ToString());
                    fileContents.Clear();
                    workspace.Undo(sourceFile.Replace(mergeArgs.SourceBranch, mergeArgs.TargetBranch));
                }
            }
        }

        /// <summary>
        /// Merges changes from a source branch to a target branch.
        /// </summary>
        /// <param name="mergeArgs">Arguments for the merge operation.</param>
        public MergeResult Merge(MergeArgs mergeArgs)
        {
            Workspace workspace = GetWorkspace(mergeArgs.TargetWorkspaceName);

            string sourceBranchPath = mergeArgs.SourceBranchPathToConsider;
            string targetBranchPath = mergeArgs.TargetBranchPathToConsider;
            VersionSpec changesetStart = new ChangesetVersionSpec(mergeArgs.ChangesetStart);
            VersionSpec changesetEnd = new ChangesetVersionSpec(mergeArgs.ChangesetEnd);

            MergeOptions mergeOption = MergeOptions.None;
            if (mergeArgs.Baseless) mergeOption = MergeOptions.Baseless;

            // pend the merge
            var result = workspace.Merge(sourceBranchPath, targetBranchPath, changesetStart, changesetEnd, LockLevel.CheckOut, RecursionType.Full, mergeOption);
            return new MergeResult()
            {
                Conflicts = result.NumConflicts,
                Failures = result.NumFailures,
                Files = result.NumFiles,
                Operations = result.NumOperations,
                Updates = result.NumUpdated,
                FailureMessages = result.NumFailures == 0 ? null : result.GetFailures().Select(f => f.Message).ToList()
            };
        }
    }
}
