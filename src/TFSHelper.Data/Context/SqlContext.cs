using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFSHelper.Data.Model;

namespace TFSHelper.Data.Context
{
    public class SqlContext : DbContext, IShelvesetManagerContext
    {
        public DbSet<Build> Builds { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<PendingChanges> PendingChanges { get; set; }
        public DbSet<Shelve> Shelves { get; set; }
        public DbSet<WorkItem> WorkItems { get; set; }
		public DbSet<CodeReview> CodeReviews { get; set; }
		public DbSet<Workspace> Workspaces { get; set; }
    }
}
