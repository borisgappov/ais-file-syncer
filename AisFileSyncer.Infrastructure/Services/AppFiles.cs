using AisFileSyncer.Infrastructure.Extensions;
using AisFileSyncer.Infrastructure.Interfaces;
using AisFileSyncer.Infrastructure.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AisFileSyncer.Infrastructure.Services
{
    public class AppFiles : IAppFiles
    {
        private const string _appDirName = "aisdata";
        private const string _filesDirName = "files";
        private const string _listFileName = "list.json";
        private readonly string _appDataDir;
        private readonly string _filesDir;
        private readonly string _listFile;
        public AppFiles()
        {
            _appDataDir = Environment.ExpandEnvironmentVariables($@"%USERPROFILE%\{_appDirName}");
            _filesDir = Path.Combine(_appDataDir, _filesDirName);
            _listFile = Path.Combine(_appDataDir, _listFileName);
            Directory.CreateDirectory(_filesDir);
        }

        public string GetAppDir()
        {
            return _appDataDir;
        }

        public string GetFilesDir()
        {
            return _filesDir;
        }

        public void DeleteFile(string FileName)
        {
            File.Delete(Path.Combine(_appDataDir, FileName));
        }

        public async Task SaveFileList(FileModel[] Files)
        {
            await File.WriteAllTextAsync(_listFile, Files.ToJSON()).ConfigureAwait(false);
        }

        public async Task<FileModel[]> ReadFileList()
        {
            return (await File.ReadAllTextAsync(_listFile).ConfigureAwait(false)).FromJSON<FileModel[]>();
        }
    }
}
