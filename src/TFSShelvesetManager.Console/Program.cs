using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TFSShelvesetManager.Core;
using TFSShelvesetManager.Core.Model;
using TFSShelvesetManager.Core.Service;

namespace TFSShelvesetManager.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            TFSManager tfs = new TFSManager();
            VersionControl vc = tfs.GetService<VersionControl>();
            ShelvingArgs shelvingArgs = new ShelvingArgs()
            {
                WorkspaceName = "WM5_WinCC_HW_Work",
                ShelvesetName = "test_shelve",
                ShelvingOption = ShelvingOption.Normal
            };
            vc.ShelvePendingChanges(shelvingArgs);

            ReadLine();
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
