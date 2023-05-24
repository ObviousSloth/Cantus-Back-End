namespace Cantus.Models
{
    public class BlobStorageDTO
    {
        public string? Uri { get; set; }
        public string? Name { get; set; }
        public string? ContentType { get; set; }
        public Stream? Content { get; set; }

    }
}
