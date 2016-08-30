using System.IO;
using System.Threading.Tasks;
using TeamBins.Common.ViewModels;

namespace TeamBins6.Infrastrucutre
{
    public interface IUploadHandler
    {
        bool IsValid(string fileName, string contentType);
        Task<UploadDto> UploadFile(string fileName, string contentType, Stream stream);
    }
}