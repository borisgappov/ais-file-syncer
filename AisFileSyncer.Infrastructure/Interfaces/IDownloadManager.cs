using AisFileSyncer.Infrastructure.Models;
using System;
using System.Threading.Tasks;

namespace AisFileSyncer.Infrastructure.Interfaces
{
    public interface IDownloadManager
    {
        FileDownloadStatus Status { get; set; }

        Task Download(FileModel[] files, Action<FileModel> downloadedCallback = null, Action completedCallback = null);

        void Cancel();
    }
}
