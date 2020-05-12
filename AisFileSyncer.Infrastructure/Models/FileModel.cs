using System;
using System.Collections.Generic;

namespace AisFileSyncer.Infrastructure.Models
{
    public class FileModel
    {
        public FileModel()
        {
            DownloadLog = new List<string>();
        }

        public string Name { get; set; }

        public Uri Uri { get; set; }

        public FileDownloadStatus DownloadStatus { get; set; }

        public List<string> DownloadLog { get; set; }

        public TimeSpan DownloadSpan { get; set; }

        public long DownloadedBytes { get; set; }

        public long Size { get; set; }

        public int Percent { get; set; }
    }
}
