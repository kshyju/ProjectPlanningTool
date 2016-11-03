using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace TeamBins.DataAccessCore
{
    public interface IEmailRepository
    {
        Task<EmailTemplateDto> GetEmailTemplate(string name);
    }

    public class EmailRepository : BaseRepo, IEmailRepository
    {
        public EmailRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<EmailTemplateDto> GetEmailTemplate(string name)
        {
            var q = @"SELECT Name,EmailBody,EmailSubject as Subject FROM EmailTemplate  WITH (NOLOCK) WHERE Name=@name";
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                var teams = await con.QueryAsync<EmailTemplateDto>(q, new { @name = name });
                return teams.FirstOrDefault();
            }
        }
    }

}