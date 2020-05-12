using AisFileSyncer.Infrastructure.Interfaces;
using AisFileSyncer.Infrastructure.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AisFileSyncer.Infrastructure.Services
{
    public class FileDownloader : IFileDownloader
    {
        private readonly IAppFiles _appFiles;

        public FileDownloader(IAppFiles appFiles)
        {
            _appFiles = appFiles;
        }

        public async Task<FileModel> Download(FileModel file, CancellationToken cancellationToken, Action<FileModel> downloadedCallback = null)
        {
            file.DownloadStatus = FileDownloadStatus.InProgress;

            var startedAt = DateTime.UtcNow;
            file.DownloadLog.Add($"{startedAt}, Download started");

            var stream = new FileStream(_appFiles.GetFilePath(file.Uri), FileMode.Create);
            try
            {
                await DownloadFileAsync(file.Uri, stream, cancellationToken, (downloadedBytes, size) =>
                {
                    file.DownloadedBytes = downloadedBytes;
                    file.Size = size;
                    downloadedCallback?.Invoke(file);
                });
                stream.Flush();
            }
            catch (TaskCanceledException ex)
            {
                file.DownloadLog.Add($"{DateTime.UtcNow}, Download cancelled");
                file.DownloadStatus = FileDownloadStatus.Cancelled;
                return file;
            }
            finally
            {
                stream.Dispose();
            }

            var finishedAt = DateTime.UtcNow;
            file.DownloadLog.Add($"{finishedAt}, Download finished");

            file.DownloadSpan = finishedAt - startedAt;

            file.DownloadStatus = FileDownloadStatus.Done;

            return file;
        }

        /// <summary>
        /// Downloads a file from the specified Uri into the specified stream.
        /// https://github.com/dotnet/runtime/issues/31479#issuecomment-578436466
        /// </summary>
        /// <param name="cancellationToken">An optional CancellationToken that can be used to cancel the in-progress download.</param>
        /// <param name="progressCallback">If not null, will be called as the download progress. The first parameter will be the number of bytes downloaded so far, and the second the total size of the expected file after download.</param>
        /// <returns>A task that is completed once the download is complete.</returns>
        public static async Task DownloadFileAsync(Uri uri, Stream toStream, CancellationToken cancellationToken = default, Action<long, long> progressCallback = null)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));
            if (toStream == null)
                throw new ArgumentNullException(nameof(toStream));

            if (uri.IsFile)
            {
                await using Stream file = File.OpenRead(uri.LocalPath);

                if (progressCallback != null)
                {
                    long length = file.Length;
                    byte[] buffer = new byte[4096];
                    int read;
                    int totalRead = 0;
                    while ((read = await file.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) > 0)
                    {
                        await toStream.WriteAsync(buffer, 0, read, cancellationToken).ConfigureAwait(false);
                        totalRead += read;
                        progressCallback(totalRead, length);
                    }
                    Debug.Assert(totalRead == length || length == -1);
                }
                else
                {
                    await file.CopyToAsync(toStream, cancellationToken).ConfigureAwait(false);
                }
            }
            else
            {
                using HttpClient client = new HttpClient();
                using HttpResponseMessage response = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                if (progressCallback != null)
                {
                    long length = response.Content.Headers.ContentLength ?? -1;
                    await using Stream stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                    byte[] buffer = new byte[4096];
                    int read;
                    int totalRead = 0;
                    while ((read = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) > 0)
                    {
                        await toStream.WriteAsync(buffer, 0, read, cancellationToken).ConfigureAwait(false);
                        totalRead += read;
                        progressCallback(totalRead, length);
                    }
                    Debug.Assert(totalRead == length || length == -1);
                }
                else
                {
                    await response.Content.CopyToAsync(toStream).ConfigureAwait(false);
                }
            }
        }
    }
}
