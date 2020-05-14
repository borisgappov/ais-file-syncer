using AisFileSyncer.Infrastructure.Interfaces;
using AisFileSyncer.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using WebWindows.Blazor;

namespace AisFileSyncer
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {

            var exceptionService = new ExceptionService();
            services.AddSingleton<IExceptionService>(exceptionService);
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                Exception e = (Exception)args.ExceptionObject;
                exceptionService.Exception = e;
            };

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
