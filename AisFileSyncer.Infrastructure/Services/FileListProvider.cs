using AisFileSyncer.Infrastructure.Interfaces;
using AisFileSyncer.Infrastructure.Models;
using AisUriProviderApi;
using System.Linq;
using System.Threading.Tasks;

namespace AisFileSyncer.Infrastructure.Services
{
    public class FileListProvider : IFileListProvider
    {
        private readonly AisUriProvider _uriProvider;
        private readonly IAppFiles _appFiles;

        public FileListProvider(IAppFiles appFiles)
        {
            _appFiles = appFiles;
            _uriProvider = new AisUriProvider();
        }

        public async Task<FileModel[]> GetUrlListAsync()
        {
            return await Task.Run(() =>
            {
                return _uriProvider
                    .Get()
                    .Select(x => new FileModel {
                        Name = _appFiles.GetFileName(x),
                        Uri = x
                    })
                    .OrderBy(x => x.Name)
                    .ToArray();
            });
        }
    }
}
