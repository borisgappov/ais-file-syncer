using AisFileSyncer.Infrastructure.Interfaces;
using AisFileSyncer.Infrastructure.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AisFileSyncer.Infrastructure.Services
{
    public class DownloadManager : IDownloadManager
    {
        private CancellationTokenSource cts;
        private readonly IFileDownloader _fileDownloader;

        public DownloadManager(IAppFiles appFiles, IFileDownloader fileDownloader)
        {
            _fileDownloader = fileDownloader;
        }

        public async Task Download(FileModel[] files, Action<FileModel> downloadedCallback = null, Action completedCallback = null)
        {
            cts = new CancellationTokenSource();

            await Task.WhenAll(files
                .AsParallel()
                .WithCancellation(cts.Token)
                .WithDegreeOfParallelism(3)
                .Select(async x =>
                {
                    return await _fileDownloader.Download(x, cts.Token, downloadedCallback);
                })
                .ToArray());

            completedCallback?.Invoke();
        }

        public void Cancel()
        {
            cts?.Cancel();
        }
    }
}
