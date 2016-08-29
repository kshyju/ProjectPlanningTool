using System;
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
        Task<int> SaveUpload(UploadDto uploadDto);
    }

    public class UploadRepository : BaseRepo, IUploadRepository
    {
        public UploadRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<int> SaveUpload(UploadDto uploadDto)
        {
            var q =
              @"INSERT INTO [dbo].[Upload](Filename,Type,Url,CreatedDate,CreatedById) VALUES(@fileName,@Type,@Url,@CreatedDate,@CreatedById);SELECT CAST(SCOPE_IDENTITY() as int)";
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                var ss = await con.QueryAsync<int>(q,uploadDto);
                return ss.Single();
            }
        }
    }
}