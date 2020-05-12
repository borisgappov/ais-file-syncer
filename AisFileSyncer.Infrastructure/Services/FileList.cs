using AisFileSyncer.Infrastructure.Extensions;
using AisFileSyncer.Infrastructure.Interfaces;
using AisFileSyncer.Infrastructure.Models;
using System.IO;
using System.Threading.Tasks;

namespace AisFileSyncer.Infrastructure.Services
{
    public class FileList : IFileList
    {
        private const string _listFileName = "list.json";
        private readonly string _listFile;
        private readonly IAppFiles _appFiles;

        public FileList(IAppFiles appFiles)
        {
            _appFiles = appFiles;
            _listFile = Path.Combine(appFiles.AppDir, _listFileName);
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
