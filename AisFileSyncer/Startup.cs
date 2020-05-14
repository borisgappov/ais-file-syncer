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
            services.AddSingleton<IFileService, FileService>();
            services.AddSingleton<IFileDownloader, FileDownloader>();
            services.AddSingleton<IContentProvider, ContentProvider>();
            services.AddSingleton<ISyncer, Syncer>();
        }

        public void Configure(DesktopApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
