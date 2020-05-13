using System;

namespace AisFileSyncer.Infrastructure.Interfaces
{
    public interface IFileService
    {

        public string AppDir { get; set; }

        public string FilesDir { get; set; }

        string GetFileName(Uri uri);

        string GetFilePath(Uri uri);

        string GetFilePath(string FileName);

        void DeleteFile(string FileName);

    }
}
