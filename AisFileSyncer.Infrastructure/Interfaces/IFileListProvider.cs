using AisFileSyncer.Infrastructure.Models;
using System.Threading.Tasks;

namespace AisFileSyncer.Infrastructure.Interfaces
{
    public interface IFileListProvider
    {
        Task<FileModel[]> GetUrlListAsync();

    }
}
