using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using TeamBins.Common.ViewModels;

namespace TeamBins6.Infrastrucutre
{
    public class LocalFileSystemStorageHandler : IUploadHandler
    {
        private readonly AppSettings settings;
        private readonly IHostingEnvironment hostingEnvironment;
        public LocalFileSystemStorageHandler(IOptions<AppSettings> settings, IHostingEnvironment hostingEnvironment)
        {
            this.settings = settings.Value;
            this.hostingEnvironment = hostingEnvironment;
        }

        public bool IsValid(string fileName, string contentType)
        {
            return (!String.IsNullOrEmpty(settings.LocalFileSystemStoragePath));
        }

        public async Task<UploadDto> UploadFile(string fileName, string contentType, Stream stream)
        {
            var uploadDirectoryPath = Path.Combine(hostingEnvironment.WebRootPath, settings.LocalFileSystemStoragePath);

            string randomFile = Path.GetFileNameWithoutExtension(fileName) +
                                "_" +
                                Guid.NewGuid().ToString().Substring(0, 4) + Path.GetExtension(fileName);


            if (!Directory.Exists(uploadDirectoryPath))
            {
                Directory.CreateDirectory(uploadDirectoryPath);
            }


            var targetFile = Path.GetFullPath(Path.Combine(uploadDirectoryPath, randomFile));

            using (var destinationStream = File.Create(targetFile))
            {
                await stream.CopyToAsync(destinationStream);
            }


            var result = new UploadDto
            {
                FileExtn = Path.GetExtension(fileName),
                FileName =  fileName,
                Url = GetFullUrl(settings.LocalFileSystemStorageUriPrefix, randomFile)
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
}