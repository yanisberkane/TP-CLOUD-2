using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using MVC.Models;

namespace MVC.Business
{
    public class BlobController
    {
        // Configuration pour recevoir les ApplicationConfiguration du AppConfig ...
        // Ici ce qui nous interesse c'est l'access au BlobConnectionString
        private ApplicationConfiguration _applicationConfiguration { get; }

        public BlobController(IOptionsSnapshot<ApplicationConfiguration> options)
        {
            _applicationConfiguration = options.Value;
        }

        public async Task<string> PushImageToBlob(IFormFile Image, Guid guid)
        {
            string blobContainerName = "images"; // The name of your blob container

            // Connect to Azurite Blob Service
            var blobServiceClient = new BlobServiceClient("UseDevelopmentStorage=true");
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(blobContainerName);

            // Create container if it doesn't exist
            await blobContainerClient.CreateIfNotExistsAsync();

            // Create a unique blob name
            string blobName = $"{guid}.jpg";

            // Upload the image to the blob container
            var blobClient = blobContainerClient.GetBlobClient(blobName);
            using (var stream = Image.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, overwrite: true);
            }

            string Url = blobClient.Uri.ToString();
            return Url;
        }
    }

    // Exception créer par la BusinessLayer pour expliquer que le fichier est trop gros.
    public class ExceptionFilesize : Exception
    { 
        public ExceptionFilesize() { }
    }
}
