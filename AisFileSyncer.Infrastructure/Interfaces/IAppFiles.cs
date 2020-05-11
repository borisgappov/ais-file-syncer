using System.Threading.Tasks;
using AisFileSyncer.Infrastructure.Models;

namespace AisFileSyncer.Infrastructure.Interfaces
{
    public interface IAppFiles
    {
        string GetAppDir();

        string GetFilesDir();

        void DeleteFile(string FileName);

        Task SaveFileList(FileModel[] Files);

        Task<FileModel[]> ReadFileList();
    }
}
