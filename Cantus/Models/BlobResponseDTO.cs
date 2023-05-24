namespace Cantus.Models
{
    public class BlobResponseDTO
    {
        public BlobResponseDTO() 
        {
            BlobStorageDTO Blob = new BlobStorageDTO();
        }
        public bool Error { get; set; }
        public string? Status { get; set; }
        public BlobStorageDTO Blob { get; set; }

    }
}
