using System.Linq;
using TeamBins.Common;

namespace TeamBins.DataAccess
{
    public class EmailTemplateRepository : IEmailTemplateRepository
    {
        private readonly TeamEntities db;
        public EmailTemplateRepository()
        {
            db = new TeamEntities();
        }
        public EmailTemplateDto GetEmailTemplate(string templateName)
        {
            var email = db.EmailTemplates.FirstOrDefault(s => s.Name == templateName);
            if (email != null)
            {
                return new EmailTemplateDto
                {
                    EmailBody = email.EmailBody,
                    Subject = email.EmailSubject,
                    TemplateName = email.Name
                };
            }
            return null;
        }
    }
}