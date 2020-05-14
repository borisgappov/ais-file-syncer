namespace AisFileSyncer.Infrastructure.Models
{
    public enum FileDownloadStatus
    {
        Waiting,
        InProgress,
        Done,
        Cancelled
    }

    public enum ContentType
    {
        Image,
        Text,
        Html,
        Unknown
    }
}
