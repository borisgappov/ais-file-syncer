using AisFileSyncer.Infrastructure.Models;
using System;
using System.Threading.Tasks;

namespace AisFileSyncer.Infrastructure.Interfaces
{

    public delegate void FileListLoadedEventHandler(FileModel[] files);

    public delegate void SyncEventHandler();

    public interface ISyncer : IDisposable
    {
        public FileModel[] files { get; set; }
        FileDownloadStatus Status { get; set; }

        public event FileListLoadedEventHandler OnFileListLoaded;

        public event SyncEventHandler OnFileDownloaded;

        public event SyncEventHandler OnAllFilesDownloaded;

        Task Sync(bool reload = true);

        void Cancel();
    }
}
