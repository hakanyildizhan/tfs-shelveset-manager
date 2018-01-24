using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace TFSShelvesetManager.Data.Cache
{
    public class DataChangeMonitor : ChangeMonitor
    {
        private string _name;
        private string _uniqueId = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);

        private static event EventHandler<DataChangeEventArgs> Signaled;

        public override string UniqueId
        {
            get
            {
                return _uniqueId;
            }
        }

        public DataChangeMonitor(string name = null)
        {
            _name = name;
            DataChangeMonitor.Signaled += OnSignaled;
            base.InitializationComplete();
        }

        private void OnSignaled(object sender, DataChangeEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.Name) || string.Compare(e.Name, _name, true) == 0)
            {
                // Cache objects are obligated to remove entry upon change notification.
                base.OnChanged(null);
            }
        }

        public static void Signal(string name = null)
        {
            if (Signaled != null)
            {
                Signaled(null, new DataChangeEventArgs(name));
            }
        }

        protected override void Dispose(bool disposing)
        {
            DataChangeMonitor.Signaled -= OnSignaled;
        }
    }

    public class DataChangeEventArgs : EventArgs
    {
        public string Name { get; private set; }
        public DataChangeEventArgs(string name = null) { this.Name = name; }
    }
}
