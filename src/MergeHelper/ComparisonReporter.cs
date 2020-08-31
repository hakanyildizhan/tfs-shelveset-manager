using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFSHelper.Core.Helpers;
using TFSHelper.Core.ViewModel;

namespace MergeHelper
{
    public class ComparisonReporter
    {
        public List<FileChangeSummary> ComparisonResults { get; set; }

        public string LogPath { get; set; }
        public string WorkitemLogPath { get; set; }

        /// <summary>
        /// Types of work items that we want to report.
        /// </summary>
        public string[] WorkItemTypes { get; set; }

        /// <summary>
        /// Reports only differences, and only the ones which have associated changesets on the source item.
        /// </summary>
        public void Report()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Converted file").Append("\t");
            sb.Append("Source file").Append("\t");
            sb.Append("Target file").Append("\t");
            sb.Append("Change").Append("\t");
            sb.Append("Changesets").Append("\t");
            sb.Append("Work items").Append("\t");
            sb.AppendLine();

            List<FileChangeSummary> changesToReport = ComparisonResults.Where(r => r.ComparisonResult != ComparisonResult.Equal && r.Changesets.Any()).ToList();

            foreach (FileChangeSummary comparison in changesToReport)
            {
                List<int> workitemIDs = new List<int>();
                comparison.Changesets.ForEach(c => c.AssociatedWorkitems.ForEach(w => 
                {
                    if (WorkItemTypes != null && WorkItemTypes.Contains(w.Type))
                        workitemIDs.Add(w.ID);
                }));

                List<int> changesetIDs = new List<int>();
                comparison.Changesets.ForEach(c => changesetIDs.Add(c.ID));

                sb.Append($"{comparison.ConvertedFilePath}").Append("\t"); // Converted file
                sb.Append($"{comparison.SourceFilePath}").Append("\t"); // Source file
                sb.Append($"{comparison.TargetFilePath}").Append("\t"); // Target file
                sb.Append($"{(comparison.ComparisonResult == ComparisonResult.DifferentInTarget ? "Different" : "New")}").Append("\t"); // Change type
                sb.Append($"{string.Join(", ", changesetIDs)}").Append("\t"); // Changesets
                sb.Append($"{string.Join(", ", workitemIDs)}").Append("\t"); // Workitems (RQs & Features)
                sb.AppendLine();
            }

            FileHelper.WriteToFile(LogPath, sb.ToString());

            // log work item report
            sb.Clear();
            sb.Append("Workitem ID").Append("\t");
            sb.Append("Type").Append("\t");
            sb.Append("Title").Append("\t");
            sb.AppendLine();

            foreach (FileChangeSummary comparison in changesToReport)
            {
                foreach (ChangesetViewModel changeset in comparison.Changesets)
                {
                    foreach (WorkitemViewModel workitem in changeset.AssociatedWorkitems)
                    {
                        sb.Append($"{workitem.ID}").Append("\t"); // Workitem ID
                        sb.Append($"{workitem.Type}").Append("\t"); // Type
                        sb.Append($"{workitem.Title}").Append("\t"); // Title
                        sb.AppendLine();
                    }
                }
            }

            FileHelper.WriteToFile(WorkitemLogPath, sb.ToString());
        }
    }
}
