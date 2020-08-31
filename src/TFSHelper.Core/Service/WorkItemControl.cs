using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TFSHelper.Core.Service
{
    internal class WorkItemControl : ITFSService
    {
        private WorkItemStore wis { get; set; }

        /// <summary>
        /// <see cref="TfsTeamProjectCollection"/> associated with the TFS service
        /// </summary>
        public TfsTeamProjectCollection ProjectCollection { get; set; }

        /// <summary>
        /// Called to initialize a TFS service implementing <see cref="ITFSService"/>
        /// </summary>
        public void Initialize()
        {
            wis = ProjectCollection.GetService<WorkItemStore>();
        }

        /// <summary>
        /// Gets a <see cref="WorkItem"/> by its ID.
        /// </summary>
        /// <param name="workItemID">ID of the work item</param>
        /// <returns></returns>
        public WorkItem GetWorkItem(int workItemID)
        {
            return wis.GetWorkItem(workItemID);
        }

        /// <summary>
        /// Gets a <see cref="ProjectCollection"/> by name.
        /// </summary>
        /// <param name="projectName">Name of the team project</param>
        /// <returns></returns>
        public Project GetTeamProject(string projectName = "TIA")
        {
            return wis.Projects[projectName];
        }

        /// <summary>
        /// Gets all links of a <see cref="WorkItem"/>.
        /// </summary>
        /// <param name="workItemId">ID of the work item</param>
        /// <returns></returns>
        public WorkItemLinkCollection GetLinks(int workItemId)
        {
            return wis.GetWorkItem(workItemId).WorkItemLinks;
        }
        
    }
}
