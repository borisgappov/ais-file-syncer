using AisFileSyncer.Infrastructure.Extensions;
using AisFileSyncer.Infrastructure.Interfaces;
using AisFileSyncer.Infrastructure.Models;
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
        public FileDownloadStatus Status { get; set; }

        private CancellationTokenSource cts;
        private readonly IFileDownloader _fileDownloader;
        private readonly IFileService _fileService;
        private readonly AisUriProvider _uriProvider;
        private readonly string _listFile;
        private FileModel[] _lastSaved = null;
        private const string _listFileName = "list.json";

        public Syncer(IFileDownloader fileDownloader, IFileService fileService)
        {
            _fileDownloader = fileDownloader;
            _fileService = fileService;
            _uriProvider = new AisUriProvider();
            _listFile = Path.Combine(fileService.AppDir, _listFileName);
        }

        public async Task Sync(FileModel[] files, Action<FileModel> downloadedCallback = null, Action completedCallback = null)
        {
            if (Status == FileDownloadStatus.InProgress)
            {
                return;
            }

            cts = new CancellationTokenSource();
            Status = FileDownloadStatus.InProgress;

            await files
                .Where(x => x.DownloadStatus != FileDownloadStatus.Done)
                .ForEachAsyncConcurrent(async x =>
                {
                    await _fileDownloader.Download(x, cts.Token, downloadedCallback);
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

            completedCallback?.Invoke();
        }

        public void Cancel()
        {
            cts?.Cancel();
        }

        public async Task<FileModel[]> GetUrlListAsync(bool reload = true)
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
    }
}
