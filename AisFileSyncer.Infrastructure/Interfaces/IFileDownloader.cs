using AisFileSyncer.Infrastructure.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AisFileSyncer.Infrastructure.Interfaces
{
    public interface IFileDownloader
    {
        Task<FileModel> Download(FileModel file, CancellationToken cancellationToken, Action<FileModel> downloadedCallback = null);
    }
}
