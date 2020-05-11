using AisFileSyncer.Infrastructure.Interfaces;
using AisFileSyncer.Infrastructure.Models;
using AisUriProviderApi;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AisFileSyncer.Infrastructure.Services
{
    public class FileProvider : IFileProvider
    {
        private readonly AisUriProvider _uriProvider;

        public FileProvider()
        {
            _uriProvider = new AisUriProvider();
        }

        public async Task<List<FileModel>> GetUrlListAsync()
        {
            return await Task.Run(() =>
            {
                return _uriProvider
                    .Get()
                    .Select(x => new FileModel {
                        Name = System.IO.Path.GetFileName(x.LocalPath),
                        Uri = x
                    })
                    .ToList();
            });
        }
    }
}
