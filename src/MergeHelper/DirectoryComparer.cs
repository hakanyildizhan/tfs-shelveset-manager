using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFSHelper.Core.Helpers;

namespace MergeHelper
{
    public class DirectoryComparer
    {
        public string SourceFolder { get; set; }
        public string TargetFolder { get; set; }
        public string FilePattern { get; set; }
        public ComparisonMode ComparisonMode { get; set; } = ComparisonMode.Text;
        private ServiceProvider _services { get; set; }

        public DirectoryComparer(ServiceProvider services)
        {
            _services = services;
        }

        public List<FileChangeSummary> Compare()
        {
            List<FileComparison> resultList = new List<FileComparison>();
            List<string> sourceFiles = new List<string>();
            if (string.IsNullOrEmpty(FilePattern))
                sourceFiles.AddRange(FileHelper.GetAllFiles(SourceFolder));
            else
                sourceFiles.AddRange(FileHelper.GetAllFilesWithPattern(SourceFolder, FilePattern));

            foreach (string sourceFile in sourceFiles)
            {
                FileComparison comparison = new FileComparison();
                comparison.SourceFilePath = sourceFile;

                if (FileHelper.FileExists(sourceFile.Replace(SourceFolder, TargetFolder)))
                {
                    comparison.TargetFilePath = sourceFile.Replace(SourceFolder, TargetFolder);
                    bool filesAreEqual = FileHelper.CompareFiles(sourceFile, sourceFile.Replace(SourceFolder, TargetFolder), ComparisonMode);

                    if (filesAreEqual)
                        comparison.ComparisonResult = ComparisonResult.Equal;
                    else
                        comparison.ComparisonResult = ComparisonResult.DifferentInTarget;
                }
                else
                {
                    comparison.ComparisonResult = ComparisonResult.DoesNotExistInTarget;
                }

                resultList.Add(comparison);
            }

            ChangeAnalyzer csAnalyzer = new ChangeAnalyzer(_services, resultList.Where(r => r.ComparisonResult != ComparisonResult.Equal).ToList());
            List<FileChangeSummary> summary = csAnalyzer.Analyze().ToList();

            return summary;
        }
    }
}
