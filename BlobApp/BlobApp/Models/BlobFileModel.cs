namespace BlobApp.Models
{
    public class BlobFileModel
    {
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long SizeInBytes { get; set; }
        public DateTimeOffset? LastModified { get; set; }

        // Helper properties
        public string SizeFormatted => SizeInBytes < 1024 * 1024
            ? $"{SizeInBytes / 1024.0:F1} KB"
            : $"{SizeInBytes / (1024.0 * 1024):F1} MB";

        public bool IsImage => ContentType.StartsWith("image/");

        public string FileIcon => ContentType switch
        {
            "application/pdf" => "📄",
            "application/msword" => "📝",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document" => "📝",
            var ct when ct.StartsWith("image/") => "🖼",
            _ => "📁"
        };
    }
}