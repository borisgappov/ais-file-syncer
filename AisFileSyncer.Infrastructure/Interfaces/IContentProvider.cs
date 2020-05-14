using AisFileSyncer.Infrastructure.Models;
using System.Threading.Tasks;

namespace AisFileSyncer.Infrastructure.Interfaces
{
    public interface IContentProvider
    {
        ContentType GetContentType(string fileName);

        Task<string> GetContent(string fileName);
    }
}
