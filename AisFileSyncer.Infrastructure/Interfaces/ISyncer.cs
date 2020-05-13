using AisFileSyncer.Infrastructure.Models;
using System;
using System.Threading.Tasks;

namespace AisFileSyncer.Infrastructure.Interfaces
{
    public interface ISyncer
    {
        FileDownloadStatus Status { get; set; }

        Task<FileModel[]> GetUrlListAsync(bool reload = true);

        Task Sync(FileModel[] files, Action<FileModel> downloadedCallback = null, Action completedCallback = null);

        void Cancel();
    }
}
