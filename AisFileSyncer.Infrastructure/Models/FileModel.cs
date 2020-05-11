using System;
using System.Collections.Generic;

namespace AisFileSyncer.Infrastructure.Models
{
    public class FileModel
    {
        public string Name { get; set; }

        public Uri Uri { get; set; }

        public FileDownloadStatus DownloadStatus { get; set; }

        public List<string> DownloadLog { get; set; }
    }
}
