namespace AisFileSyncer.Infrastructure.Models
{
    public class PreviewContent
    {
        public PreviewContent()
        {
            contentType = ContentType.Unknown;
            content = string.Empty;
        }
        public ContentType contentType { get; set; }

        public string content { get; set; }
    }
}
