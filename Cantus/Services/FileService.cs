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
        


      /*  public FileService(string storageAccounts, BlobContainerClient filesContainer, IConfiguration config)
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
            
        }*/
        public FileService()
        {
            var _storageAccounts = "";//_config["AzureBlobStorageDetails:StorageAccount:Name"];
            var _key = "";//_config["AzureBlobStorageDetails:StorageAccount:Key"];
            //var _storageAccounts = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build().GetSection("AzureBlobStorageDetails:StorageAccount")["Name"];
            //var _key = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build().GetSection("AzureBlobStorageDetails:StorageAccount")["Key"];
            var credential = new StorageSharedKeyCredential(_storageAccounts, _key);
            var blobUri = $"https://{_storageAccounts}.blob.core.windows.net";
            var _blobServiceClient = new BlobServiceClient(new Uri(blobUri), credential);
            _filesContainer = _blobServiceClient.GetBlobContainerClient("music");

        }

  
        public async Task<BlobResponseDTO> UploadMusicAsync(IFormFile blob)
        {
            BlobResponseDTO response = new();
            
            try
            {
                BlobClient client = _filesContainer.GetBlobClient($"Songs/{blob.FileName}");
                await using (Stream? data = blob.OpenReadStream())
                {
                    await client.UploadAsync(data);
                }
                response.Status = $"File {blob.FileName} uploaded successfully";
                response.Error = false;
                response.Blob.Uri = client.Uri.AbsoluteUri;
                response.Blob.Name = client.Name;
                return response;

            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Status = $"File {blob.FileName} upload unsuccessfull: {ex.Message}";
                return response;
            }
            
        }
        public async Task<BlobResponseDTO> UploadFilesAsync(IFormFile blob)
        {
            BlobResponseDTO response = new();

            try
            {
                BlobClient client = _filesContainer.GetBlobClient(blob.FileName);
                await using (Stream? data = blob.OpenReadStream())
                {
                    await client.UploadAsync(data);
                }
                response.Status = $"File {blob.FileName} uploaded successfully";
                response.Error = false;
                response.Blob.Uri = client.Uri.AbsoluteUri;
                response.Blob.Name = client.Name;
                return response;

            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Status = $"File {blob.FileName} upload unsuccessfull: {ex.Message}";
                return response;
            }

        }
        public async Task<List<BlobStorageDTO>> ListAsync()
        {
            List<BlobStorageDTO> files = new List<BlobStorageDTO>();
            try
            {
                await foreach (var file in _filesContainer.GetBlobsAsync())
                {
                    string uri = _filesContainer.Uri.ToString();
                    var name = file.Name;
                    var fullUri = $"{uri}/{name}";

                    files.Add(new BlobStorageDTO
                    {
                        Uri = fullUri,
                        Name = name,
                        ContentType = file.Properties.ContentType
                    });
                }
                return files;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cannot get blobs {ex.Message}");
                throw;
            }
        }

        public async Task<BlobStorageDTO> DownloadAsync(string blobFileName)
        {
            BlobClient file = _filesContainer.GetBlobClient(blobFileName);
            try
            {
                if (await file.ExistsAsync())
                {
                    var data = await file.OpenReadAsync();
                    Stream blobContent = data;

                    var content = await file.DownloadContentAsync();

                    string name = blobFileName;
                    string contentType = content.Value.Details.ContentType;

                    return new BlobStorageDTO
                    {
                        Content = blobContent,
                        Name = name,
                        ContentType = contentType
                    };
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }

        }
         
        public async Task<BlobResponseDTO> DeleteAsync(string blobFileName)
        {
            try
            {
                BlobClient file = _filesContainer.GetBlobClient(blobFileName);

                await file.DeleteAsync();
                return new BlobResponseDTO
                {
                    Error = false,
                    Status = $"File {blobFileName} has been deleted",

                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
            
        }

    }
}
