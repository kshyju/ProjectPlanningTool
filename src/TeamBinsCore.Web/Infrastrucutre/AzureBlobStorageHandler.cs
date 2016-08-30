using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using TeamBins.Common.ViewModels;

namespace TeamBins6.Infrastrucutre
{
    public class AzureBlobStorageHandler : IUploadHandler
    {

        private const string UploadContainer = "teambins-issue-uploads";
        private readonly IConfiguration configuration;
        AppSettings settings;
        // private IConfiguration configuration;
        public AzureBlobStorageHandler(IOptions<AppSettings> settings)
        {

            this.settings = settings.Value;
        }

        public bool IsValid(string fileName, string contentType)
        {
            return !String.IsNullOrEmpty(this.settings.AzureblobStorageConnectionString);
        }

        public async Task<UploadDto> UploadFile(string fileName, string contentType, Stream stream)
        {
            // CloudConfigurationManager.
            var account = CloudStorageAccount.Parse(settings.AzureblobStorageConnectionString);
            var client = account.CreateCloudBlobClient();
            var container = client.GetContainerReference(UploadContainer);

            // Randomize the filename everytime so we don't overwrite files
            string randomFile = Path.GetFileNameWithoutExtension(fileName) +
                                "_" +
                                Guid.NewGuid().ToString().Substring(0, 4) + Path.GetExtension(fileName);

            await container.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Blob, new BlobRequestOptions(), new OperationContext());



            CloudBlockBlob blockBlob = container.GetBlockBlobReference(randomFile);
            blockBlob.Properties.ContentType = contentType;

            await blockBlob.UploadFromStreamAsync(stream);

            //  await Task.Factory.FromAsync((cb, state) => blockBlob.UploadFromStreamAsync(stream, cb, state), ar => blockBlob.EndUploadFromStream(ar), null);

            var result = new UploadDto
            {
                FileName = fileName,
                Url = blockBlob.Uri.ToString(),
            };

            return result;
        }
    }
}