using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFSHelper.Core.Service;

namespace MergeHelper
{
    public class Application2
    {
        private IVersionControl _versionControl { get; set; }
        private ILogger _logger { get; set; }

        public Application2(IServiceCollection serviceCollection)
        {
            var services = serviceCollection.BuildServiceProvider();
            _versionControl = services.GetRequiredService<IVersionControl>();
            _logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("Application2");
        }

        public void Start()
        {
            //Console.Write("TFS file/folder path to get changesets? ");
            //string basePath = Console.ReadLine();

            Console.Write("Source branch? ");
            string sourceBranch = Console.ReadLine();

            Console.Write("Target branch? ");
            string targetBranch = Console.ReadLine();

            Console.Write("Target workspace name? ");
            string targetWorkspace = Console.ReadLine();

            Console.Write("Starting changeset? ");
            int startChangeset = int.Parse(Console.ReadLine());

            //var changesetsToMerge = _versionControl.GetChangesetsForPath(basePath)?.OrderBy(a => a).Where(a => a >= startChangeset).ToList();
            var changesetsToMerge = new List<int> { startChangeset };

            Console.Write($"Found {changesetsToMerge.Count} changesets. ");
            Console.ReadLine();

            foreach (var cs in changesetsToMerge)
            {
                Console.Clear();
                Console.Write($"Merging changeset {cs}. Continue? ");
                Console.ReadLine();

                var result = _versionControl.Merge(new TFSHelper.Core.Model.MergeArgs()
                {
                    Baseless = true,
                    ChangesetStart = cs,
                    ChangesetEnd = cs,
                    SourceBranchPathToConsider = sourceBranch,
                    TargetBranchPathToConsider = targetBranch,
                    TargetWorkspaceName = targetWorkspace
                });

                Console.WriteLine($"Results: ");
                Console.WriteLine($"***********");
                Console.WriteLine($"Conflicts: {result.Conflicts}");
                Console.WriteLine($"Failures: {result.Failures}");
                Console.WriteLine($"Files: {result.Files}");
                Console.WriteLine($"Operations: {result.Operations}");
                Console.WriteLine($"Updates: {result.Updates}");

                if (result.FailureMessages != null && result.FailureMessages.Any())
                {
                    Console.WriteLine();
                    Console.WriteLine("Failures: ");

                    foreach (var msg in result.FailureMessages)
                    {
                        Console.WriteLine(msg);
                    }
                }

                Console.WriteLine();
                Console.Write($"Press Enter to continue. ");
                Console.ReadLine();
            }

            //var result = _versionControl.Merge(new TFSHelper.Core.Model.MergeArgs()
            //{
            //    Baseless = true,
            //    ChangesetStart = startChangeset,
            //    ChangesetEnd = changesetsToMerge.OrderByDescending(a => a).First(),
            //    SourceBranchPathToConsider = sourceBranch,
            //    TargetBranchPathToConsider = targetBranch,
            //    TargetWorkspaceName = targetWorkspace
            //});

            //Console.WriteLine($"Results: ");
            //Console.WriteLine($"***********");
            //Console.WriteLine($"Conflicts: {result.Conflicts}");
            //Console.WriteLine($"Failures: {result.Failures}");
            //Console.WriteLine($"Files: {result.Files}");
            //Console.WriteLine($"Operations: {result.Operations}");
            //Console.WriteLine($"Updates: {result.Updates}");

            //if (result.FailureMessages != null && result.FailureMessages.Any())
            //{
            //    Console.WriteLine();
            //    Console.WriteLine("Failures: ");

            //    foreach (var msg in result.FailureMessages)
            //    {
            //        Console.WriteLine(msg);
            //    }
            //}

            Console.Write("Finished. ");
            Console.ReadLine();
        }
    }
}
