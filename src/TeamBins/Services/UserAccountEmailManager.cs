using TeamBins.Common;
using TeamBins.Common.Infrastructure;
using TeamBins.DataAccess;
using TechiesWeb.TeamBins.Infrastructure;

namespace TeamBins.Services
{
    public class UserAccountEmailManager : IUserAccountEmailManager
    {
        readonly IEmailTemplateRepository repository;
        public UserAccountEmailManager(IEmailTemplateRepository repository)
        {
            this.repository = repository;
        }
        public void SendResetPasswordEmail(UserAccountDto userAccount,string activationLink)
        {
            var emailTemplate = repository.GetEmailTemplate("ResetPassword");
            if (emailTemplate != null)
            {
                string emailSubject = emailTemplate.Subject;
                string emailBody = emailTemplate.EmailBody;
                Email email = new Email();
                email.ToAddress.Add(userAccount.EmailAddress);

                string joinLink = $"{UrlBuilderHelper.SiteBaseUrl}Account/resetpassword/{activationLink}";
                emailBody = emailBody.Replace("@resetLink", joinLink);
                email.Body = emailBody;
                email.Subject = emailSubject;
                email.Send();
            }
        }
    }
}