using System.IO;
using AisFileSyncer.Infrastructure.Interfaces;
using AisFileSyncer.Infrastructure.Models;
using AisUriProviderApi;
using System.Linq;
using System.Threading.Tasks;
using AisFileSyncer.Infrastructure.Extensions;

namespace AisFileSyncer.Infrastructure.Services
{
    public class FileListProvider : IFileListProvider
    {
        private readonly AisUriProvider _uriProvider;
        private readonly IAppFiles _appFiles;
        private readonly IFileList _fileList;

        public FileListProvider(IAppFiles appFiles, IFileList fileList)
        {
            _appFiles = appFiles;
            _fileList = fileList;
            _uriProvider = new AisUriProvider();
        }

        public async Task<FileModel[]> GetUrlListAsync(bool reload = true)
        {

            var previous = (await _fileList.GetFileList().ConfigureAwait(false))
                .Where(x => x.DownloadStatus == FileDownloadStatus.Done && File.Exists(_appFiles.GetFilePath(x.Name)))
                .ToArray();

            if (!reload && previous.Count(x => x.DownloadStatus == FileDownloadStatus.Done) > 0)
            {
                return previous;
            }

            var files = await Task.Run(() =>
            {
                return _uriProvider
                    .Get()
                    .Select(x => new FileModel {
                        Name = _appFiles.GetFileName(x),
                        Uri = x
                    })
                    .OrderBy(x => Path.GetFileNameWithoutExtension(x.Name).Length)
                    .ThenBy(x => x.Name)
                    .ToArray();
            }).ConfigureAwait(false);

            files = (from f in files
                    from p in previous.Where(x => x.Name == f.Name).DefaultIfEmpty()
                    select new { f, p })
                .Select(x =>
                {
                    var file = x.p == null ? x.f : x.p.Clone();
                    file.DownloadStatus = x.p == null
                        ? FileDownloadStatus.Waiting
                        : FileDownloadStatus.Done;
                    return file;
                }).ToArray();

            return files;
        }
    }
}
