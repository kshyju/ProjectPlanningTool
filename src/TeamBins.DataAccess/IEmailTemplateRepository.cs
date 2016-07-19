using TeamBins.Common;

namespace TeamBins.DataAccess
{
    public interface IEmailTemplateRepository
    {
        EmailTemplateDto GetEmailTemplate(string templateName);
    }
}