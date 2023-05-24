using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
namespace Cantus.Services
{
    public class BlobStorageService
    {
        public BlobServiceClient GetBlobServiceClient(string accountName)
        {
            BlobServiceClient client = new(
                new Uri($"https://{accountName}.blob.core.windows.net"),
                new DefaultAzureCredential());

            return client;
        }
    }
}
