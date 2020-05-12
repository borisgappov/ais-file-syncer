using AisFileSyncer.Infrastructure.Extensions;
using AisFileSyncer.Infrastructure.Interfaces;
using AisFileSyncer.Infrastructure.Models;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AisFileSyncer.Infrastructure.Services
{
    public class FileList : IFileList
    {
        private const string _listFileName = "list.json";
        private readonly string _listFile;
        private FileModel[] _lastSaved = null;

        public FileList(IAppFiles appFiles)
        {
            _listFile = Path.Combine(appFiles.AppDir, _listFileName);
        }

        public async Task SaveFileList(FileModel[] files)
        {
            _lastSaved = files.Where(x => x.DownloadStatus == FileDownloadStatus.Done).ToArray();
            await File.WriteAllTextAsync(_listFile, _lastSaved.ToJSON()).ConfigureAwait(false);
        }

        public async Task<FileModel[]> ReadFileList()
        {
            var emptyResult = new FileModel[] { };
            return File.Exists(_listFile)
                ? (await File.ReadAllTextAsync(_listFile).ConfigureAwait(false)).FromJSON(emptyResult)
                : emptyResult;
        }

        public async Task<FileModel[]> GetFileList()
        {
            if (_lastSaved == null)
            {
                _lastSaved = await ReadFileList();
            }

            return _lastSaved;
        }
    }
}
