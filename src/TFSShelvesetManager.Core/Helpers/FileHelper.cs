using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFSShelvesetManager.Core;

namespace TFSShelvesetManager.Core.Helpers
{
    public class FileHelper
    {
        /// <summary>
        /// Creates the folders to store application artifacts, if they do not exist.
        /// </summary>
        public static void EnsureArtifactFolders()
        {
            Preferences.Default.ArtifactFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TFSShelvesetManager");
            Preferences.Default.WorkspaceFolder = Path.Combine(Preferences.Default.ArtifactFolder, "Workspaces");

            foreach (SettingsPropertyValue setting in Preferences.Default.PropertyValues.Cast<SettingsPropertyValue>().ToList())
            {
                Directory.CreateDirectory(setting.PropertyValue.ToString());
            }
        }

        /// <summary>
        /// Deletes a folder recursively.
        /// </summary>
        /// <param name="folderPath">Path of the folder.</param>
        public static void DeleteFolder(string folderPath)
        {
            Directory.Delete(folderPath, true);
        }

        /// <summary>
        /// Creates a folder.
        /// </summary>
        /// <param name="folderPath">Path of the folder to create.</param>
        public static void CreateFolder(string folderPath)
        {
            Directory.CreateDirectory(folderPath);
        }
    }
}
