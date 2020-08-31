using Microsoft.TeamFoundation.VersionControl.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFSHelper.Core.Helpers;
using TFSHelper.Core.Model;
using TFSHelper.Core.Service;

namespace TFSHelper.Core.ViewModel
{
    public class ShelvesetViewModel
    {
        private VersionControl _vc;
        private WorkItemControl _wis;
        public ShelvesetViewModel()
        {
            TFSManager tfs = new TFSManager();
            _vc = tfs.GetService<VersionControl>();
            _wis = tfs.GetService<WorkItemControl>();
        }
        public void UpdateShelvesetTest(string shelvesetName)
        {
            Workspace ws = CreateTemporaryWorkspace();

            ShelvingArgs shelveArgs = new ShelvingArgs()
            {
                WorkspaceName = ws.Name,
                ShelvesetName = shelvesetName
            };

            _vc.Unshelve(shelveArgs);
            
            Shelveset shelveset = _vc.GetShelveSet(shelvesetName);
            shelveArgs.Comment = shelveset.Comment;

            WorkItemCheckinInfo[] workItemInfo = new WorkItemCheckinInfo[] {
                new WorkItemCheckinInfo(_wis.GetWorkItem(2138190), WorkItemCheckinAction.Associate),
                new WorkItemCheckinInfo(_wis.GetWorkItem(2784451), WorkItemCheckinAction.Associate)
            };
            //shelveset.WorkItemInfo = workItemInfo;
            shelveArgs.AssociatedWorkItemInfo = workItemInfo;

            shelveArgs.ShelvingOption = ShelvingOption.Replace;
            _vc.ShelvePendingChanges(shelveArgs);
            _vc.DeleteWorkspace(shelveArgs.WorkspaceName);
            FileHelper.DeleteFolder(Path.Combine(Preferences.Default.WorkspaceFolder, ws.Name));
        }

        private Workspace CreateTemporaryWorkspace()
        {
            string tempWorkspaceName = "_tempworkspace_" + DateTime.Now.ToString("yyyMMdd-HHmmss");

            WorkspaceArgs args = new WorkspaceArgs()
            {
                WorkspaceName = tempWorkspaceName,
                ServerItemPath = "$/",
                LocalPath = Path.Combine(Preferences.Default.WorkspaceFolder, tempWorkspaceName)
            };
            return _vc.CreateWorkspace(args);
        }
    }
}
