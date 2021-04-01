using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.Services
{
    public class AzureFileStorage : IFileStorage
    {
        private readonly string connectionString;

        public AzureFileStorage(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("AzureStorage");
        }

        public async Task DeleteFile(string path, string container)
        {
           if (path == null)
            {
                return;
            }

            var account = CloudStorageAccount.Parse(connectionString);
            var client = account.CreateCloudBlobClient();
            var containerRef = client.GetContainerReference(container);
            var blobName = Path.GetFileName(path);
            var blob = containerRef.GetBlobReference(blobName);
            await blob.DeleteIfExistsAsync();
        }

        public async Task<string> EditFile(byte[] content, string extension, string container, string path, string contentType)
        {
            await DeleteFile(path, container);
            return await SaveFile(content, extension, container, contentType);
        }

        public async Task<string> SaveFile(byte[] content, string extension, string container, string contentType)
        {
            var account = CloudStorageAccount.Parse(connectionString);
            var client = account.CreateCloudBlobClient();
            var containerRef = client.GetContainerReference(container);
            await containerRef.CreateIfNotExistsAsync();
            await containerRef.SetPermissionsAsync(new Microsoft.WindowsAzure.Storage.Blob.BlobContainerPermissions
            {
                PublicAccess = Microsoft.WindowsAzure.Storage.Blob.BlobContainerPublicAccessType.Blob
            });

            var fileName = $"{Guid.NewGuid()}{extension}";
            var blob = containerRef.GetBlockBlobReference(fileName);
            await blob.UploadFromByteArrayAsync(content, 0, content.Length);
            await blob.SetPropertiesAsync();
            return blob.Uri.ToString();
        }
    }
}
