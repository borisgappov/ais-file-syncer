using AisFileSyncer.Infrastructure.Interfaces;
using AisFileSyncer.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using WebWindows.Blazor;

namespace AisFileSyncer
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IAppFiles, AppFiles>();
            services.AddSingleton<IFileList, FileList>();
            services.AddSingleton<IFileDownloader, FileDownloader>();
            services.AddSingleton<IDownloadManager, DownloadManager>();
            services.AddSingleton<IFileListProvider, FileListProvider>();
        }

        public void Configure(DesktopApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
