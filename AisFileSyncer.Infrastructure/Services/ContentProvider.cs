using AisFileSyncer.Infrastructure.Interfaces;
using AisFileSyncer.Infrastructure.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AisFileSyncer.Infrastructure.Services
{
    public class ContentProvider : IContentProvider
    {
        private readonly IFileService _fileService;
        public ContentProvider(IFileService fileService)
        {
            _fileService = fileService;
        }

        public ContentType GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            switch (extension)
            {
                case ".jpg":
                case ".gif":
                case ".png":
                case ".bmp":
                    return ContentType.Image;
                case ".txt":
                case ".css":
                case ".js":
                    return ContentType.Text;
                case ".html":
                    return ContentType.Html;
                default:
                    return ContentType.Unknown;
            }
        }

        public async Task<string> GetContent(string fileName)
        {
            var contentType = GetContentType(fileName);
            var filePath = _fileService.GetFilePath(fileName);
            switch (contentType)
            {
                case ContentType.Image:
                    var imageBytes = await File.ReadAllBytesAsync(filePath);
                    var base64data = Convert.ToBase64String(imageBytes);
                    var extension = Path.GetExtension(fileName).Replace(".", "");
                    return $"data:image/{extension};base64,{base64data}";
                case ContentType.Text:
                    return await File.ReadAllTextAsync(filePath);
                case ContentType.Html:
                    return await File.ReadAllTextAsync(filePath);
            }
            return string.Empty;
        }
    }
}
