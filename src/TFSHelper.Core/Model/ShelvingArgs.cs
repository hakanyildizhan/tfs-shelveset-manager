using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace TFSHelper.Core.Model
{
    public class ShelvingArgs
    {
        /// <summary>
        /// Name of the <see cref="Workspace"/> to perform the shelving operation for.
        /// </summary>
        public string WorkspaceName { get; set; }

        /// <summary>
        /// Name of the <see cref="Shelveset"/>.
        /// </summary>
        public string ShelvesetName { get; set; }

        /// <summary>
        /// How to perform this shelving action.
        /// </summary>
        public ShelvingOption ShelvingOption { get; set; }

        /// <summary>
        /// Work item info to associate with the <see cref="Shelveset"/>.
        /// </summary>
        public WorkItemCheckinInfo[] AssociatedWorkItemInfo { get; set; }

        /// <summary>
        /// Remarks about the changes in the <see cref="Shelveset"/>.
        /// </summary>
        public string Comment { get; set; }
    }

    /// <summary>
    /// Options for how to perform the shelving operation
    /// </summary>
    public enum ShelvingOption
    {
        /// <summary>
        /// Perform a normal shelve operation. Prompts if the shelveset already exists and leaves the pending changes in your workspace.
        /// </summary>
        Normal,

        /// <summary>
        /// After the shelve operation has completed, undo the change from the local workspace. This moves the pending changes from your workspace to the shelveset.
        /// </summary>
        UndoPendingChanges,

        /// <summary>
        /// Forces the new shelveset to overwrite the existing shelveset without prompting.
        /// </summary>
        Replace
    }
}
