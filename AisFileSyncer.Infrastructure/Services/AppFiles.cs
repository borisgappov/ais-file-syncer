using AisFileSyncer.Infrastructure.Interfaces;
using System;
using System.IO;

namespace AisFileSyncer.Infrastructure.Services
{
    public class AppFiles : IAppFiles
    {
        private const string _appDirName = "aisdata";
        private const string _filesDirName = "files";

        public string AppDir { get; set; }
        public string FilesDir { get; set; }

        public AppFiles()
        {
            AppDir = Environment.ExpandEnvironmentVariables($@"%USERPROFILE%\{_appDirName}");
            FilesDir = Path.Combine(AppDir, _filesDirName);
            Directory.CreateDirectory(FilesDir);
        }

        public string GetFilePath(string FileName)
        {
            return Path.Combine(FilesDir, FileName);
        }

        public void DeleteFile(string FileName)
        {
            try
            {
                File.Delete(GetFilePath(FileName));
            }
            catch
            {
                // log here
            }
        }

        public string GetFileName(Uri uri)
        {
            return Path.GetFileName(uri.LocalPath);
        }

        public string GetFilePath(Uri uri)
        {
            return GetFilePath(GetFileName(uri));
        }
    }
}
