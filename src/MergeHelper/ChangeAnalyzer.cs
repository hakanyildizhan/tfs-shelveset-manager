using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFSHelper.Core.Service;
using TFSHelper.Core.ViewModel;
using TFSHelper.Core;

namespace MergeHelper
{
    public class ChangeAnalyzer
    {
        private IVersionControl _vc { get; set; }
        private int _startingChangeset { get; set; }
        private List<FileComparison> _comparisonResults { get; set; }
        
        public ChangeAnalyzer(ServiceProvider services, List<FileComparison> comparisonResults)
        {
            _comparisonResults = comparisonResults;
            _vc = services.GetRequiredService<IVersionControl>();
            _startingChangeset = Preferences.Default.CHANGESET_START;
        }

        public IEnumerable<FileChangeSummary> Analyze()
        {
            foreach (FileComparison comparison in _comparisonResults)
            {
                string originalFilePath = comparison.SourceFilePath.Replace(Preferences.Default.COMPARISON_SOURCE_FOLDER, Preferences.Default.HSP_XDD_PATH).Replace(".imp", "");

                // check if there is ANY changeset, linked to a work item or not
                // which is higher than our starting changeset
                List<ChangesetViewModel> changesets = _vc.GetChangesetsForFile(originalFilePath, _startingChangeset);
                if (changesets.Any() && changesets.OrderByDescending(c => c.ID).FirstOrDefault().ID > Preferences.Default.CHANGESET_START)
                {
                    ////
                    //changesets = changesets.AsEnumerable().FilterByItemType(new string[] { "Feature", "Request" }).ToList();

                    //// ignore if there has been no change since CHANGESET_START
                    //if (!changesets.Any() || changesets.OrderByDescending(c => c.ID).FirstOrDefault().ID < Preferences.Default.CHANGESET_START) continue;

                    yield return new FileChangeSummary()
                    {
                        ConvertedFilePath = comparison.SourceFilePath,
                        SourceFilePath = originalFilePath,
                        TargetFilePath = comparison.TargetFilePath,
                        ComparisonResult = comparison.ComparisonResult,
                        Changesets = changesets.Where(c => c.ID > Preferences.Default.CHANGESET_START).ToList()
                    };
                }
            }
            
        }
    }
}
