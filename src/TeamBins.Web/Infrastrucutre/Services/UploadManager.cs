using System;
using System.Threading.Tasks;
using TeamBins.Common.ViewModels;
using TeamBins.DataAccessCore;

namespace TeamBins.Services
{
    public interface IUploadManager
    {
        Task<int> SaveUpload(UploadDto uploadDto);
    }
    public class UploadManager : IUploadManager
    {
        private IUploadRepository uploadRepository;
        public UploadManager(IUploadRepository uploadRepository)
        {
            this.uploadRepository = uploadRepository;
        }
        public async Task<int> SaveUpload(UploadDto uploadDto)
        {
            uploadDto.CreatedDate = DateTime.UtcNow;
            return await uploadRepository.SaveUpload(uploadDto);
        }
    }
}