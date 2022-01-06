using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFSHelper.Core.Model;
using TFSHelper.Core.ViewModel;

namespace TFSHelper.Core.Service
{
    public interface IVersionControl : ITFSService
    {
        Shelveset GetShelveSet(string shelvesetName, string owner);
        Shelveset GetShelveSet(string shelvesetName);
        List<Shelveset> GetShelveSets();
        void UpdateShelveSet(string shelvesetName, Shelveset updatedShelveSet);
        Dictionary<string, object> GetShelvesetProperties(string shelvesetName);
        List<Workspace> GetLocalWorkspaces();
        List<WorkspaceInfo> GetLocalWorkspaceInfo();
        void ShelvePendingChanges(ShelvingArgs shelvingArguments);
        void Unshelve(ShelvingArgs shelvingArguments);
        void UnshelveAdvanced(ShelvingArgs shelvingArguments);
        void GetLatest(string path);
        Workspace CreateWorkspace(WorkspaceArgs workspaceArgs);
        void DeleteWorkspace(string workspaceName);
        void MergeBetweenVersions(MergeArgs mergeArgs);
        MergeResult Merge(MergeArgs mergeArgs);
        List<ChangesetViewModel> GetChangesetsForFile(string localFilePath, int startingChangesetNumber = 0, string[] workitemTypeFilter = null);
        List<int> GetChangesetsForPath(string path);
        int GetLastChangesetIDForPath(string path);
        void CheckoutFiles(List<string> fileList);
        UndoPendingChangesResult UndoPendingChanges(string localPath);
        void AddFilesToSourceControl(List<string> fileList);
        List<string> GetLocalItemsNotUnderSourceControl(string directory);
    }
}
