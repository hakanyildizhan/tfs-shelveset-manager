using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TFSShelvesetManager.Core;
using TFSShelvesetManager.Core.Helpers;
using TFSShelvesetManager.Core.Model;
using TFSShelvesetManager.Core.Service;
using TFSShelvesetManager.Core.ViewModel;

namespace TFSShelvesetManager.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            FileHelper.EnsureArtifactFolders();
            //TFSManager tfs = new TFSManager();
            //VersionControl vc = tfs.GetService<VersionControl>();
            //ShelvingArgs shelvingArgs = new ShelvingArgs()
            //{
            //    WorkspaceName = "WM5_WinCC_HW_Work",
            //    ShelvesetName = "test_shelve",
            //    ShelvingOption = ShelvingOption.Normal
            //};
            //vc.ShelvePendingChanges(shelvingArgs);

            //ShelvesetViewModel shelvesetVM = new ShelvesetViewModel();
            //shelvesetVM.UpdateShelvesetTest("RQ2138190_work");

            // baseless merge
            TFSManager tfs = new TFSManager();
            VersionControl vc = tfs.GetService<VersionControl>();

            MergeBetweenHSPBranches(vc);
            MergeBetweenHSPBranchesCSharp(vc);

            WriteLine("Finished.");
            ReadLine();
        }

        private static void MergeBetweenHSPBranchesCSharp(VersionControl vc)
        {
            MergeArgs mergeArgs = new MergeArgs()
            {
                Baseless = true,
                ChangesetStart = 2646671,
                ChangesetEnd = 2669705,
                SourceBranch = "TH_HSP1500_All",
                TargetBranch = "TH_HSP150001_All",
                SourceBranchPathToConsider = "$/TIA/_Src/HSP/dev/TH_HSP1500_All/src/HM/HSP/HSP_V15_0238_001_Softstarter_3RW5",
                TargetBranchPathToConsider = "$/TIA/_Src/HSP/dev/TH_HSP150001_All/src/HM/HSP/HSP_V15_1_0238_001_Softstarter_3RW5",
                TargetWorkspacePath = @"D:\WS\TH_HSP150001_All",
                TargetWorkspaceName = "TH_HSP150001_All"
            };

            vc.Merge(mergeArgs);
        }

        private static void MergeBetweenHSPBranches(VersionControl vc)
        {
            MergeArgs mergeArgs = new MergeArgs()
            {
                Baseless = true,
                ChangesetStart = 2677810,
                ChangesetEnd = 2693253,
                SourceBranch = "TH_HSP1500_All",
                TargetBranch = "TH_HSP150001_All",
                SourceBranchPathToConsider = "$/TIA/_Src/HSP/dev/TH_HSP1500_All/src/HM/Xdd/Sirius",
                TargetBranchPathToConsider = "$/TIA/_Src/HSP/dev/TH_HSP150001_All/src/HM/Xdd/Sirius",
                TargetWorkspacePath = @"D:\WS\TH_HSP150001_All",
                TargetWorkspaceName = "TH_HSP150001_All",
                SourceMDDVersion = "V15.0.0.0",
                TargetMDDVersion = "V15.10.0.0"
            };

            vc.MergeBetweenVersions(mergeArgs);
        }

        static void WriteLine(string lineToWrite)
        {
            System.Console.WriteLine(lineToWrite);
        }

        static void ReadLine()
        {
            System.Console.ReadLine();
        }
    }
}
