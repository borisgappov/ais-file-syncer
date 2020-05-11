using AisFileSyncer.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AisFileSyncer.Infrastructure.Interfaces
{
    public interface IFileProvider
    {
        Task<List<FileModel>> GetUrlListAsync();

    }
}
