using System.Threading.Tasks;
using TeamBins.Common;

namespace TeamBins.DataAccess
{
    public interface IEmailRepository
    {
        Task<EmailTemplateDto> GetEmailTemplate(string name);
    }
}