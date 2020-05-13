using AisFileSyncer.Infrastructure.Extensions;
using AisFileSyncer.Infrastructure.Interfaces;
using AisFileSyncer.Infrastructure.Models;
using AisFileSyncer.Infrastructure.Utils;
using AisUriProviderApi;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AisFileSyncer.Infrastructure.Services
{
    public class Syncer : ISyncer
    {
        public FileModel[] files { get; set; }
        public FileDownloadStatus Status { get; set; }
        private CancellationTokenSource cts;
        private readonly IFileDownloader _fileDownloader;
        private readonly IFileService _fileService;
        private readonly AisUriProvider _uriProvider;
        private readonly string _listFile;
        private FileModel[] _lastSaved = null;
        private SyncTimer _timer = new SyncTimer();
        private const string _listFileName = "list.json";
        private const double _interval = 1000 * 60 * 5; // 5 min

        public event FileListLoadedEventHandler OnFileListLoaded;

        public event SyncEventHandler OnFileDownloaded;

        public event SyncEventHandler OnAllFilesDownloaded;

        public Syncer(IFileDownloader fileDownloader, IFileService fileService)
        {
            _fileDownloader = fileDownloader;
            _fileService = fileService;
            _uriProvider = new AisUriProvider();
            _listFile = Path.Combine(fileService.AppDir, _listFileName);
            _timer.Start(async () => await Sync(), _interval);
        }

        public async Task Sync(bool reload = true)
        {
            if (Status == FileDownloadStatus.InProgress)
            {
                return;
            }

            cts = new CancellationTokenSource();
            Status = FileDownloadStatus.InProgress;

            files = await GetUrlListAsync(reload);

            OnFileListLoaded?.Invoke(files);

            await files
                .Where(x => x.DownloadStatus != FileDownloadStatus.Done)
                .ForEachAsyncConcurrent(async x =>
                {
                    await _fileDownloader.Download(x, cts.Token, (f) => { OnFileDownloaded?.Invoke(); });
                }, 3);

            Status = cts.IsCancellationRequested
                ? FileDownloadStatus.Cancelled
                : FileDownloadStatus.Done;

            await SaveFileList(files);

            var filesToDelete =
                (from fileName in new DirectoryInfo(_fileService.FilesDir).GetFiles().Select(x => x.Name).ToArray()
                 from f in files.Where(x => x.Name == fileName && x.DownloadStatus == FileDownloadStatus.Done).DefaultIfEmpty()
                 where f == null
                 select fileName).ToArray();

            foreach (var fileName in filesToDelete)
            {
                _fileService.DeleteFile(fileName);
            }

            OnAllFilesDownloaded?.Invoke();
        }

        public void Cancel()
        {
            cts?.Cancel();
        }

        private async Task<FileModel[]> GetUrlListAsync(bool reload = true)
        {
            var previous = (await GetFileList())
                .Where(x => x.DownloadStatus == FileDownloadStatus.Done && File.Exists(_fileService.GetFilePath(x.Name)))
                .ToArray();

            if (!reload && previous.Count(x => x.DownloadStatus == FileDownloadStatus.Done) > 0)
            {
                return previous;
            }

            var files = await Task.Run(() =>
            {
                return _uriProvider
                    .Get()
                    .Select(x => new FileModel
                    {
                        Name = _fileService.GetFileName(x),
                        Uri = x
                    })
                    .OrderBy(x => Path.GetFileNameWithoutExtension(x.Name).Length)
                    .ThenBy(x => x.Name)
                    .ToArray();
            });

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

        private async Task SaveFileList(FileModel[] files)
        {
            _lastSaved = files.Where(x => x.DownloadStatus == FileDownloadStatus.Done).ToArray();
            await File.WriteAllTextAsync(_listFile, _lastSaved.ToJSON()).ConfigureAwait(false);
        }

        private async Task<FileModel[]> GetFileList()
        {
            if (_lastSaved == null)
            {
                var emptyResult = new FileModel[] { };
                _lastSaved = File.Exists(_listFile)
                    ? (await File.ReadAllTextAsync(_listFile).ConfigureAwait(false)).FromJSON(emptyResult)
                    : emptyResult;
            }

            return _lastSaved;
        }

        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}
