using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TFSHelper.Core;

namespace TFSHelper.Core.Helpers
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
            if (folderPath.ToLower().Equals("c:") || folderPath.ToLower().Equals(@"c:\") || folderPath.ToLower().Contains(@"c:\windows"))
                throw new UnauthorizedAccessException("This folder cannot be deleted.");
            else if (!DirectoryExists(folderPath))
                return;

            string[] files = Directory.GetFiles(folderPath);
            string[] dirs = Directory.GetDirectories(folderPath);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteFolder(dir);
            }

            Directory.Delete(folderPath, false);
        }

        /// <summary>
        /// Deletes given file or folder (recursively).
        /// </summary>
        /// <param name="filePath"></param>
        public static void DeleteItem(string path)
        {
            if (path.ToLower().Contains(@"c:\windows"))
                throw new UnauthorizedAccessException("This file cannot be deleted.");
            else if (!FileExists(path) && !DirectoryExists(path))
                return;

            if (FileExists(path))
            {
                File.SetAttributes(path, FileAttributes.Normal);
                File.Delete(path);
            }
            else if (DirectoryExists(path))
            {
                DeleteFolder(path);
            }
        }

        /// <summary>
        /// Creates a folder.
        /// </summary>
        /// <param name="folderPath">Path of the folder to create.</param>
        public static void CreateFolder(string folderPath)
        {
            Directory.CreateDirectory(folderPath);
        }

        public static bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public static string GetParentDirectory(string path)
        {
            if (FileExists(path) || Directory.Exists(path))
                return Directory.GetParent(path).FullName;
            else
                return string.Empty;
        }

        /// <summary>
        /// Compares two files and returns true if two files are equal; otherwise returns false.
        /// </summary>
        /// <param name="filePath1"></param>
        /// <param name="filePath2"></param>
        /// <param name="comparisonMode"></param>
        /// <returns></returns>
        public static bool CompareFiles(string filePath1, string filePath2, ComparisonMode comparisonMode)
        {
            switch (comparisonMode)
            {
                case ComparisonMode.Text:
                    return CompareContents(filePath1, filePath2);
                case ComparisonMode.Binary:
                    return CompareBinaryFiles(filePath1, filePath2);
                default:
                    return true;
            }
        }

        /// <summary>
        /// Returns true if two binary files are the same.
        /// </summary>
        /// <param name="filePath1"></param>
        /// <param name="filePath2"></param>
        /// <returns></returns>
        private static bool CompareBinaryFiles(string filePath1, string filePath2)
        {
            if (!File.Exists(filePath1) || !File.Exists(filePath2))
                return false;

            // Check the file size and CRC equality
            using (var file1 = new FileStream(filePath1, FileMode.Open))
            using (var file2 = new FileStream(filePath2, FileMode.Open))
                return FileStreamEquals(file1, file2);
        }

        static bool FileStreamEquals(Stream stream1, Stream stream2)
        {
            const int bufferSize = 2048;
            byte[] buffer1 = new byte[bufferSize]; //buffer size
            byte[] buffer2 = new byte[bufferSize];
            while (true)
            {
                int count1 = stream1.Read(buffer1, 0, bufferSize);
                int count2 = stream2.Read(buffer2, 0, bufferSize);

                if (count1 != count2)
                    return false;

                if (count1 == 0)
                    return true;

                if (!buffer1.Take(count1).SequenceEqual(buffer2.Take(count2)))
                    return false;
            }
        }

        /// <summary>
        /// Returns true if the contents of two files are the same excluding whitespace.
        /// </summary>
        /// <param name="filePath1"></param>
        /// <param name="filePath2"></param>
        /// <returns></returns>
        public static bool CompareContents(string filePath1, string filePath2)
        {
            if (FileExists(filePath1) && FileExists(filePath2))
            {
                string normalized1 = Regex.Replace(File.ReadAllText(filePath1), @"\s", "");
                string normalized2 = Regex.Replace(File.ReadAllText(filePath2), @"\s", "");

                return string.Equals(
                    normalized1,
                    normalized2,
                    StringComparison.OrdinalIgnoreCase);
            }
            else
                return false;
        }

        public static string GetCurrentFolder()
        {
            return GetParentDirectory(Assembly.GetExecutingAssembly().Location);
        }

        public static void WriteToFile(string path, string contents)
        {
            File.WriteAllText(path, contents);
        }

        public static string ReadFile(string path)
        {
            if (FileExists(path))
                return File.ReadAllText(path);
            else
                return null;
        }

        public static List<string> GetAllFiles(string path)
        {
            if (Directory.Exists(path))
                return Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).ToList();
            else
                return null;
        }

        public static List<string> GetAllFilesWithPattern(string path, string pattern)
        {
            if (Directory.Exists(path))
                return Directory.GetFiles(path, pattern, SearchOption.AllDirectories).ToList();
            else
                return null;
        }

        public static void PasteFolderContents(string sourcePath, string targetPath)
        {
            if (DirectoryExists(sourcePath) && DirectoryExists(targetPath))
            {
                string[] sourceFileList = Directory.GetFiles(sourcePath);
                for (int i = 0; i < sourceFileList.Count(); i++)
                    File.Move(sourceFileList[i], sourceFileList[i].Replace(sourcePath, targetPath));
            }
        }

        /// <summary>
        /// Writes source file contents into corresponding target files.
        /// </summary>
        /// <param name="filePairList"></param>
        public static void PasteFileContents(Dictionary<string, string> filePairList)
        {
            foreach (var filePair in filePairList)
            {
                if (FileExists(filePair.Key) && FileExists(filePair.Value))
                    WriteToFile(filePair.Value, ReadFile(filePair.Key));
            }
        }

        /// <summary>
        /// Copies a file into the target path.
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="targetPath"></param>
        public static void CopyFile(string sourcePath, string targetPath)
        {
            if (FileExists(sourcePath))
                File.Copy(sourcePath, targetPath);
        }

        public static string CombinePaths(string path1, string path2)
        {
            return Path.Combine(path1, path2);
        }

        public static string CombinePaths(string path1, string path2, string path3)
        {
            return Path.Combine(new string[] { path1, path2, path3 });
        }

        public static string GetFileName(string path)
        {
            return new FileInfo(path).Name;
        }

        public static bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }
    }
}
