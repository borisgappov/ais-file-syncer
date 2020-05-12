using AisFileSyncer.Infrastructure.Extensions;
using AisFileSyncer.Infrastructure.Interfaces;
using AisFileSyncer.Infrastructure.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AisFileSyncer.Infrastructure.Services
{
    public class DownloadManager : IDownloadManager
    {
        public FileDownloadStatus Status { get; set; }

        private CancellationTokenSource cts;
        private readonly IFileDownloader _fileDownloader;
        private readonly IFileList _fileList;
        private readonly IAppFiles _appFiles;

        public DownloadManager(IFileDownloader fileDownloader, IFileList fileList, IAppFiles appFiles)
        {
            _fileDownloader = fileDownloader;
            _fileList = fileList;
            _appFiles = appFiles;
        }

        public async Task Download(FileModel[] files, Action<FileModel> downloadedCallback = null, Action completedCallback = null)
        {
            if (Status == FileDownloadStatus.InProgress)
            {
                return;
            }

            cts = new CancellationTokenSource();
            Status = FileDownloadStatus.InProgress;

            await files
                .Where(x => x.DownloadStatus != FileDownloadStatus.Done)
                .ForEachAsyncConcurrent(async x => {
                    await _fileDownloader.Download(x, cts.Token, downloadedCallback);
                }, 3);

            Status = cts.IsCancellationRequested
                ? FileDownloadStatus.Cancelled
                : FileDownloadStatus.Done;

            await _fileList.SaveFileList(files).ConfigureAwait(false);

            var filesToDelete =
                (from fileName in new DirectoryInfo(_appFiles.FilesDir).GetFiles().Select(x => x.Name).ToArray()
                 from f in files.Where(x => x.Name == fileName && x.DownloadStatus == FileDownloadStatus.Done).DefaultIfEmpty()
                 where f == null
                 select fileName).ToArray();

            foreach (var fileName in filesToDelete)
            {
                _appFiles.DeleteFile(fileName);
            }

            completedCallback?.Invoke();
        }

        public void Cancel()
        {
            cts?.Cancel();
        }
    }
}
