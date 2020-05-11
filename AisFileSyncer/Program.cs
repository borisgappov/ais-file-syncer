using WebWindows.Blazor;
using System;

namespace AisFileSyncer
{
    public class Program
    {
        static void Main(string[] args)
        {
            ComponentsDesktop.Run<Startup>("Ais File Syncer", "wwwroot/index.html");
        }
    }
}
