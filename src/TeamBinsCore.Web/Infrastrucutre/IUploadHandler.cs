using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using TeamBins.Common.ViewModels;

namespace TeamBins6.Infrastrucutre
{
    public interface IUploadHandler
    {
        bool IsValid(string fileName, string contentType);
        Task<UploadResult> UploadFile(string fileName, string contentType, Stream stream);
    }
    public class LocalFileSystemStorageHandler : IUploadHandler
    {
        AppSettings settings;
        private IConfiguration configuration;
        public LocalFileSystemStorageHandler(IOptions<AppSettings> settings)
        {
            
            this.settings = settings.Value;
        }

        public bool IsValid(string fileName, string contentType)
        {
            return (!String.IsNullOrEmpty(settings.LocalFileSystemStoragePath));
        }

        public async Task<UploadResult> UploadFile(string fileName, string contentType, Stream stream)
        {
            string randomFile = Path.GetFileNameWithoutExtension(fileName) +
                                "_" +
                                Guid.NewGuid().ToString().Substring(0, 4) + Path.GetExtension(fileName);


            if (!Directory.Exists(settings.LocalFileSystemStoragePath))
            {
                Directory.CreateDirectory(settings.LocalFileSystemStoragePath);
            }

           
            string targetFile = Path.GetFullPath(Path.Combine(settings.LocalFileSystemStoragePath, randomFile));

            using (FileStream destinationStream = File.Create(targetFile))
            {
                await stream.CopyToAsync(destinationStream);
            }


            var result = new UploadResult
            {
                Url = GetFullUrl(settings.LocalFileSystemStorageUriPrefix, randomFile),
                Identifier = randomFile
            };

            return result;
        }

        private string GetFullUrl(string prefix, string fileName)
        {
            if (!prefix.EndsWith("/"))
            {
                prefix += "/";
            }

            var prefixUri = new Uri(prefix);
            return new Uri(prefixUri, fileName).ToString();
        }
    }

    //public class AzureBlobStorageHandler : IUploadHandler
    //{

    //    private const string JabbRUploadContainer = "teambins-issue-uploads";
    //    private readonly IConfiguration configuration;
    //    public AzureBlobStorageHandler(IConfiguration configuration)
    //    {
    //        this.configuration = configuration;
    //    }

    //    public bool IsValid(string fileName, string contentType)
    //    {
    //        // Blob storage can handle any content
    //        return true;
    //    }

    //    public async Task<UploadResult> UploadFile(string fileName, string contentType, Stream stream)
    //    {
    //        var account = CloudStorageAccount.Parse(_settingsFunc().AzureblobStorageConnectionString);
    //        var client = account.CreateCloudBlobClient();
    //        var container = client.GetContainerReference(JabbRUploadContainer);

    //        // Randomize the filename everytime so we don't overwrite files
    //        string randomFile = Path.GetFileNameWithoutExtension(fileName) +
    //                            "_" +
    //                            Guid.NewGuid().ToString().Substring(0, 4) + Path.GetExtension(fileName);

    //        if (container.CreateIfNotExists())
    //        {
    //            // We need this to make files servable from blob storage
    //            container.SetPermissions(new BlobContainerPermissions
    //            {
    //                PublicAccess = BlobContainerPublicAccessType.Blob
    //            });
    //        }

    //        CloudBlockBlob blockBlob = container.GetBlockBlobReference(randomFile);
    //        blockBlob.Properties.ContentType = contentType;

    //        await Task.Factory.FromAsync((cb, state) => blockBlob.BeginUploadFromStream(stream, cb, state), ar => blockBlob.EndUploadFromStream(ar), null);

    //        var result = new UploadResult
    //        {
    //            Url = blockBlob.Uri.ToString(),
    //            Identifier = randomFile
    //        };

    //        return result;
    //    }
    //}

}