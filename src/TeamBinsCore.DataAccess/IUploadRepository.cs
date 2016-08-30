using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using TeamBins.Common.ViewModels;

namespace TeamBins.DataAccessCore
{
    public interface IUploadRepository
    {
        Task<IEnumerable<UploadDto>> GetUploads(int parentId);
        Task<int> SaveUpload(UploadDto uploadDto);
    }

    public class UploadRepository : BaseRepo, IUploadRepository
    {
        public UploadRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<IEnumerable<UploadDto>> GetUploads(int parentId)
        {
            var q = @" SELECT * FROM  [dbo].[Upload] WHERE ParentId=@parentId";
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                return await con.QueryAsync<UploadDto>(q,new { parentId });
            }
        }

        public async Task<int> SaveUpload(UploadDto uploadDto)
        {
            var q =
              @"INSERT INTO [dbo].[Upload](Filename,Type,Url,CreatedDate,CreatedById,ParentId) VALUES(@fileName,@Type,@Url,@CreatedDate,@CreatedById,@ParentId);SELECT CAST(SCOPE_IDENTITY() as int)";
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                var ss = await con.QueryAsync<int>(q,uploadDto);
                return ss.Single();
            }
        }
    }
}