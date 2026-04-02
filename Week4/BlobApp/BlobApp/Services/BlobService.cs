using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BlobApp.Models;

namespace BlobApp.Services
{
    public class BlobService
    {
        private readonly BlobContainerClient _container;

        public BlobService(IConfiguration config)
        {
            var connectionString = config["AzureBlob:ConnectionString"];
            var containerName = config["AzureBlob:ContainerName"];
            _container = new BlobContainerClient(connectionString, containerName);
        }

        // ── LIST ALL FILES ────────────────────────────────────────────────
        public async Task<List<BlobFileModel>> ListFilesAsync()
        {
            var files = new List<BlobFileModel>();

            await foreach (var blob in _container.GetBlobsAsync())
            {
                var blobClient = _container.GetBlobClient(blob.Name);

                files.Add(new BlobFileModel
                {
                    Name = blob.Name,
                    Url = blobClient.Uri.ToString(),
                    ContentType = blob.Properties.ContentType ?? "application/octet-stream",
                    SizeInBytes = blob.Properties.ContentLength ?? 0,
                    LastModified = blob.Properties.LastModified
                });
            }

            return files;
        }

        // ── UPLOAD FILE ───────────────────────────────────────────────────
        public async Task<bool> UploadFileAsync(IFormFile file)
        {
            try
            {
                var blobClient = _container.GetBlobClient(file.FileName);

                var uploadOptions = new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders
                    {
                        ContentType = file.ContentType
                    }
                };

                using var stream = file.OpenReadStream();
                await blobClient.UploadAsync(stream, uploadOptions);
                return true;
            }
            catch { return false; }
        }

        // ── DOWNLOAD FILE ─────────────────────────────────────────────────
        public async Task<(Stream stream, string contentType, string fileName)> DownloadFileAsync(string fileName)
        {
            var blobClient = _container.GetBlobClient(fileName);
            var downloadRes = await blobClient.DownloadAsync();

            return (
                downloadRes.Value.Content,
                downloadRes.Value.ContentType,
                fileName
            );
        }

        // ── DELETE FILE ───────────────────────────────────────────────────
        public async Task<bool> DeleteFileAsync(string fileName)
        {
            try
            {
                var blobClient = _container.GetBlobClient(fileName);
                await blobClient.DeleteIfExistsAsync();
                return true;
            }
            catch { return false; }
        }
    }
}