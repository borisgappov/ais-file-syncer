using AisFileSyncer.Infrastructure.Models;
using System.Threading.Tasks;

namespace AisFileSyncer.Infrastructure.Interfaces
{
    public interface IFileList
    {
        Task SaveFileList(FileModel[] files);

        Task<FileModel[]> ReadFileList();

        Task<FileModel[]> GetFileList();
    }
}
