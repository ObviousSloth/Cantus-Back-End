using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Cantus.Models;
using Microsoft.Build.Framework;
using Microsoft.Extensions.Configuration;

namespace Cantus.Services
{
    public class FileService
    {
       
      
        private readonly BlobContainerClient _filesContainer;
        private readonly IConfiguration _config;
        private readonly BlobServiceClient _blobServiceClient;

        public FileService(string storageAccounts, BlobContainerClient filesContainer, IConfiguration config)
        {
            _config = config;
            var _storageAccounts = _config["AzureBlobStorageDetails:StorageAccount:Name"];
            var _key = _config["AzureBlobStorageDetails:StorageAccount:Key"];
            //var _storageAccounts = new ConfigurationBuilder().AddJsonFile("appsettings.Developement.json").Build().GetSection("AzureBlobStorageDetails:StorageAccount")["Name"];
            //var _key = new ConfigurationBuilder().AddJsonFile("appsettings.Developement.json").Build().GetSection("AzureBlobStorageDetails:StorageAccount")["Key"];
            var credential = new StorageSharedKeyCredential(_storageAccounts, _key);
            var blobUri = $"https://{_storageAccounts}.blob.core.windows.net";
            _blobServiceClient = new BlobServiceClient(new Uri(blobUri), credential);
            _filesContainer = _blobServiceClient.GetBlobContainerClient("music");
            
        }
        public FileService()
        {
          
            var _storageAccounts = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build().GetSection("AzureBlobStorageDetails:StorageAccount")["Name"];
            var _key = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build().GetSection("AzureBlobStorageDetails:StorageAccount")["Key"];
            var credential = new StorageSharedKeyCredential(_storageAccounts, _key);
            var blobUri = $"https://{_storageAccounts}.blob.core.windows.net";
            _blobServiceClient = new BlobServiceClient(new Uri(blobUri), credential);
            _filesContainer = _blobServiceClient.GetBlobContainerClient("music");

        }

        public async Task ListBlobContainersAsync()
        {
            var containers = _blobServiceClient.GetBlobContainersAsync();

            await foreach (var container in containers)
            {
                Console.WriteLine(container.Name);
            }
        }

        public async Task<List<Uri>> UploadFilesAsync()
        {
            var blobUris = new List<Uri>();
            string filePath = "hello.txt";
            var blobContainer = _blobServiceClient.GetBlobContainerClient("music");

            var blob = blobContainer.GetBlobClient($"today/{filePath}");
            var tommorowBlob = blobContainer.GetBlobClient($"tommorow/{filePath}");

            await blob.UploadAsync(filePath, true);
            blobUris.Add(blob.Uri);
            await tommorowBlob.UploadAsync(filePath, true);
            blobUris.Add(tommorowBlob.Uri);

            return blobUris;
        }
        public async Task<List<BlobStorageDTO>> ListAs

    }
}
