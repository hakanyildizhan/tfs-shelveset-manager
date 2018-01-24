﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFSShelvesetManager.Data.Model.Base;

namespace TFSShelvesetManager.Data.Model.ObjectModel
{
    public class WorkItem : IWorkItem
    {
        public int WorkItemID { get; set; }
        public string AssignedTo { get; set; }
        public string Name { get; set; }
        public virtual ICollection<WorkItem> WorkItemLinks { get; set; }
        public WorkItemStatus WorkItemStatus { get; set; }
    }
}