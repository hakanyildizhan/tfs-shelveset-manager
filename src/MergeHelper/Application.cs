using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using TFSHelper.Core.Helpers;
using TFSHelper.Core.Service;
using TFSHelper.Core.ViewModel;

namespace MergeHelper
{
    public class Application
    {
        private IVersionControl _versionControl { get; set; }
        private ILogger _logger { get; set; }
        private ServiceProvider _services { get; set; }

        public Application(IServiceCollection serviceCollection)
        {
            _services = serviceCollection.BuildServiceProvider();
            _versionControl = _services.GetRequiredService<IVersionControl>();
            _logger = _services.GetRequiredService<ILoggerFactory>().CreateLogger("Application");
            Initialize();
        }

        private void Initialize()
        {
            Preferences.Default.Reset();

            Console.Write("Starting changeset number? ");
            Preferences.Default.CHANGESET_START = int.Parse(Console.ReadLine());
            Console.Write($"Integration package file path (Press Enter to accept default: {Preferences.Default.PACKAGE_PATH})? ");
            string input = Console.ReadLine();
            if (!string.IsNullOrEmpty(input))
            {
                while (!FileHelper.FileExists(input))
                {
                    Console.Write($"File not found. Please try again: ");
                    input = Console.ReadLine();
                }
                Preferences.Default.PACKAGE_PATH = input;
                Preferences.Default.Save();
            }

            Preferences.Default.PACKAGE_NAME = FileHelper.GetFileName(Preferences.Default.PACKAGE_PATH);
            Preferences.Default.HSP_XDD_PATH = FileHelper.GetParentDirectory(Preferences.Default.PACKAGE_PATH);

            Console.Write($"TIA Base branch local path (Press Enter to accept default: {Preferences.Default.WM5_PATH})? ");
            input = Console.ReadLine();
            if (!string.IsNullOrEmpty(input))
            {
                while (!FileHelper.DirectoryExists(input))
                {
                    Console.Write($"Directory not found. Please try again: ");
                    input = Console.ReadLine();
                }
                Preferences.Default.WM5_PATH = input;
                Preferences.Default.Save();
            }

            Console.Write($"HSP branch local path (Press Enter to accept default: {Preferences.Default.HSP_PATH})? ");
            input = Console.ReadLine();
            if (!string.IsNullOrEmpty(input))
            {
                while (!FileHelper.DirectoryExists(input))
                {
                    Console.Write($"Directory not found. Please try again: ");
                    input = Console.ReadLine();
                }
                Preferences.Default.HSP_PATH = input;
                Preferences.Default.Save();
            }

            Console.Write($"Tooltip file pattern (Press Enter to accept default: {Preferences.Default.TOOLTIP_FILE_PATTERN})? ");
            input = Console.ReadLine();
            if (!string.IsNullOrEmpty(input))
            {
                Preferences.Default.TOOLTIP_FILE_PATTERN = input;
                Preferences.Default.Save();
            }
                
            Preferences.Default.MDD_CONVERTER_EXE = FileHelper.CombinePaths(Preferences.Default.WM5_PATH, Preferences.Default.MDD_CONVERTER_EXE);
            Preferences.Default.MDD_CONVERTER_FOLDER = FileHelper.GetParentDirectory(Preferences.Default.MDD_CONVERTER_EXE);

            Preferences.Default.COMPARISON_SOURCE_FOLDER = FileHelper.CombinePaths(FileHelper.GetParentDirectory(FileHelper.GetParentDirectory(Preferences.Default.PACKAGE_PATH)), "xddsh");
            Preferences.Default.HSP_XDDSHPASS1_PATH = FileHelper.CombinePaths(FileHelper.GetParentDirectory(FileHelper.GetParentDirectory(Preferences.Default.PACKAGE_PATH)), "xddshPass1");
            Preferences.Default.COMPARISON_TARGET_FOLDER = FileHelper.CombinePaths(Preferences.Default.WM5_PATH, Preferences.Default.COMPARISON_TARGET_FOLDER);

            Preferences.Default.HSP_TOOLTIPS_ENGLISH = FileHelper.CombinePaths(Preferences.Default.HSP_PATH, Preferences.Default.HSP_TOOLTIPS_ENGLISH);
            Preferences.Default.HSP_TOOLTIPS_OTHER = FileHelper.CombinePaths(Preferences.Default.HSP_PATH, Preferences.Default.HSP_TOOLTIPS_OTHER);
            Preferences.Default.WM5_TOOLTIPS_ENGLISH = FileHelper.CombinePaths(Preferences.Default.WM5_PATH, Preferences.Default.WM5_TOOLTIPS_ENGLISH);
            Preferences.Default.WM5_TOOLTIPS_OTHER = FileHelper.CombinePaths(Preferences.Default.WM5_PATH, Preferences.Default.WM5_TOOLTIPS_OTHER);

            Preferences.Default.Save();
        }

        internal void Start()
        {
            Stopwatch watch = Stopwatch.StartNew();

            #region Cleanup & Get latest
            _logger.LogInformation("Deleting any existing converter tool artifacts..");
            FileHelper.DeleteFolder(Preferences.Default.COMPARISON_SOURCE_FOLDER);
            FileHelper.DeleteFolder(Preferences.Default.HSP_XDDSHPASS1_PATH);
            _logger.LogInformation("Done.");

            _logger.LogInformation("Getting latest from the HSP branch..");
            _versionControl.GetLatest(Preferences.Default.HSP_XDD_PATH);
            _logger.LogInformation("Done.");

            _logger.LogInformation("Getting latest from the WM5 branch..");
            _versionControl.GetLatest(Preferences.Default.COMPARISON_TARGET_FOLDER);
            _logger.LogInformation("Done.");

            _logger.LogInformation("Getting latest from the WM5 branch (converter tool)..");
            _versionControl.GetLatest(Preferences.Default.MDD_CONVERTER_FOLDER);
            _logger.LogInformation("Done.");
            #endregion

            // save current changeset
            Preferences.Default.CHANGESET_END = _versionControl.GetLastChangesetIDForPath(Preferences.Default.HSP_XDD_PATH);
            Preferences.Default.Save();
            _logger.LogInformation($"Current changeset on source branch: {Preferences.Default.CHANGESET_END}");

            #region Convert
            _logger.LogInformation("Converting files..");
            Process process = new Process();

            _logger.LogInformation("Getting latest from the TIA Base branch for the MDD converter..");
            _versionControl.GetLatest(FileHelper.GetParentDirectory(Preferences.Default.MDD_CONVERTER_EXE));
            _logger.LogInformation("Done.");
            
            // step 1
            process.StartInfo = new ProcessStartInfo()
            {
                FileName = Preferences.Default.MDD_CONVERTER_EXE,
                Arguments = $"-c \"{Preferences.Default.PACKAGE_PATH}\" -pass1c",
                WorkingDirectory = Preferences.Default.MDD_CONVERTER_FOLDER,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            process.Start();
            process.WaitForExit();

            // step 2
            process = new Process();
            process.StartInfo = new ProcessStartInfo()
            {
                FileName = Preferences.Default.MDD_CONVERTER_EXE,
                Arguments = $"-c \"{FileHelper.CombinePaths(FileHelper.GetParentDirectory(Preferences.Default.HSP_XDD_PATH), "xddshPass1", Preferences.Default.PACKAGE_NAME)}\"",
                WorkingDirectory = Preferences.Default.MDD_CONVERTER_FOLDER,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            process.Start();
            process.WaitForExit();
            _logger.LogInformation("Done.");

            // check if folders with NameClash0_ prefix are created
            if (FileHelper.DirectoryExists(FileHelper.CombinePaths(Preferences.Default.COMPARISON_SOURCE_FOLDER, "NameClash0__systemglobal")))
            {
                FileHelper.PasteFolderContents(FileHelper.CombinePaths(Preferences.Default.COMPARISON_SOURCE_FOLDER, "NameClash0__systemglobal"), FileHelper.CombinePaths(Preferences.Default.COMPARISON_SOURCE_FOLDER, "_systemglobal"));
                FileHelper.PasteFolderContents(FileHelper.CombinePaths(Preferences.Default.COMPARISON_SOURCE_FOLDER, "NameClash0_Sirius"), FileHelper.CombinePaths(Preferences.Default.COMPARISON_SOURCE_FOLDER, "Sirius"));
            }
            #endregion

            #region Compare
            _logger.LogInformation($"Deleting local items that are not added to source control on the target path..");
            List<string> localItems = _versionControl.GetLocalItemsNotUnderSourceControl(Preferences.Default.COMPARISON_TARGET_FOLDER);
            localItems.ForEach(i => FileHelper.DeleteItem(i));
            _logger.LogInformation("Done.");

            _logger.LogInformation("Reporting..");
            DirectoryComparer comparer = new DirectoryComparer(_services);
            // compare MDD files
            comparer.SourceFolder = Preferences.Default.COMPARISON_SOURCE_FOLDER;
            comparer.TargetFolder = Preferences.Default.COMPARISON_TARGET_FOLDER;
            List<FileChangeSummary> comparisonResult = comparer.Compare();

            // compare tooltip files
            // binary file comparison needs improvement - currently crashes on newFile copy operation
            // we should not do anything about files that has not been changed
            /*
            comparer.SourceFolder = Preferences.Default.HSP_TOOLTIPS_ENGLISH;
            comparer.TargetFolder = Preferences.Default.WM5_TOOLTIPS_ENGLISH;
            comparer.ComparisonMode = ComparisonMode.Binary;
            comparer.FilePattern = Preferences.Default.TOOLTIP_FILE_PATTERN;
            comparisonResult.AddRange(comparer.Compare());
            comparer.SourceFolder = Preferences.Default.HSP_TOOLTIPS_OTHER;
            comparer.TargetFolder = Preferences.Default.WM5_TOOLTIPS_OTHER;
            comparisonResult.AddRange(comparer.Compare());
            */

            ComparisonReporter reporter = new ComparisonReporter();
            reporter.ComparisonResults = comparisonResult.Where(c => !c.SourceFilePath.Equals(Preferences.Default.PACKAGE_PATH)).ToList();
            reporter.LogPath = FileHelper.CombinePaths(FileHelper.GetCurrentFolder(), "log.csv");
            reporter.WorkitemLogPath = FileHelper.CombinePaths(FileHelper.GetCurrentFolder(), "log_workitems.csv");
            reporter.WorkItemTypes = new string[] { "Feature", "Request" };
            reporter.Report();
            _logger.LogInformation($"Done. Change report location: {reporter.LogPath}");
            _logger.LogInformation($"Workitem report location: {reporter.WorkitemLogPath}");
            #endregion

            #region Check and notify if tooltip files have changed
            Console.ForegroundColor = ConsoleColor.Red;
            int lastCSTooltipEN = _versionControl.GetLastChangesetIDForPath(Preferences.Default.HSP_TOOLTIPS_ENGLISH);
            int lastCSTooltipOTHER = _versionControl.GetLastChangesetIDForPath(Preferences.Default.HSP_TOOLTIPS_OTHER);
            if (lastCSTooltipEN > Preferences.Default.CHANGESET_START || lastCSTooltipOTHER > Preferences.Default.CHANGESET_START)
                Console.WriteLine("Warning: There are changes in tooltip files!");
            Console.ForegroundColor = ConsoleColor.White;
            #endregion

            // edit/add target files
            Thread.Sleep(1000);
            Console.Write("Do you want to check out target files & apply the changes / add new items (y/n)? ");
            ConsoleKeyInfo input = Console.ReadKey();
            Console.WriteLine();
            while (input.Key != ConsoleKey.Y && input.Key != ConsoleKey.N)
            {
                Console.Write("Invalid input, enter again (y/n): ");
                input = Console.ReadKey();
                Console.WriteLine();
            }
            
            if (input.Key == ConsoleKey.N)
                _logger.LogInformation($"Changes are not applied.");
            else
            {
                _logger.LogInformation("Your changes are applied on target branch..");
                UndoPendingChangesResult result = _versionControl.UndoPendingChanges(Preferences.Default.WM5_PATH);

                if (result == UndoPendingChangesResult.Failure)
                    _logger.LogInformation("An error occurred while undoing your changes.");
                else if (result == UndoPendingChangesResult.PartialSuccess)
                    _logger.LogInformation("Some of your changes could not be undone.");
                else if (result == UndoPendingChangesResult.NoAction)
                    _logger.LogInformation("No pending change detected.");
                else
                    _logger.LogInformation("Your changes are successfully undone.");

                // existing files
                _versionControl.CheckoutFiles(comparisonResult.Where(c => c.ComparisonResult == ComparisonResult.DifferentInTarget && c.Changesets.Any()).Select(c => c.TargetFilePath).ToList());
                Dictionary<string, string> sourceTargetFilePairs = new Dictionary<string, string>();
                comparisonResult.Where(c => c.ComparisonResult == ComparisonResult.DifferentInTarget && c.Changesets.Any()).ToList().ForEach(c => sourceTargetFilePairs.Add(c.ConvertedFilePath, c.TargetFilePath));
                FileHelper.PasteFileContents(sourceTargetFilePairs);

                // new files
                List<string> newFiles = comparisonResult.Where(c => c.ComparisonResult == ComparisonResult.DoesNotExistInTarget).Select(c => c.ConvertedFilePath).ToList();
                newFiles.ForEach(f => FileHelper.CopyFile(f, f.Replace(Preferences.Default.COMPARISON_SOURCE_FOLDER, Preferences.Default.COMPARISON_TARGET_FOLDER)));
                _versionControl.AddFilesToSourceControl(newFiles.Select(f => f.Replace(Preferences.Default.COMPARISON_SOURCE_FOLDER, Preferences.Default.COMPARISON_TARGET_FOLDER)).ToList());
            }

            watch.Stop();
            _logger.LogInformation($"Duration: {(watch.ElapsedMilliseconds / 1000) / 60} minutes {(watch.ElapsedMilliseconds % (((watch.ElapsedMilliseconds / 1000) / 60) * 60000)) / 1000} seconds");
            Thread.Sleep(1000);
        }
    }
}
