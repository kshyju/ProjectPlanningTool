using System;
using System.Net;
using System.Threading.Tasks;
using TeamBins.Common;

using Microsoft.Extensions.Configuration;


using TeamBins.DataAccessCore;
using TeamBins.Common.ViewModels;

namespace TeamBins.Services
{
    public interface IEmailManager
    {
        Task SendTeamMemberInvitationEmail(AddTeamMemberRequestVM teamMemberRequest);

    }

    public class EmailManager : IEmailManager
    {
        private IEmailRepository emailRepository;
        private IConfiguration configuration;
        public EmailManager(IEmailRepository emailRepository, IConfiguration configuration)
        {
            this.emailRepository = emailRepository;
            this.configuration = configuration;
        }

        public async Task SendTeamMemberInvitationEmail(AddTeamMemberRequestVM teamMemberRequest)
        {
            var emailTemplate = await emailRepository.GetEmailTemplate("JoinMyTeam");
            if (emailTemplate != null)
            {
                
               
                string emailSubject = emailTemplate.Subject;
                string emailBody = emailTemplate.EmailBody;
                Email email = new Email();
                email.ToAddress.Add(teamMemberRequest.EmailAddress);



                string joinLink = String.Format("{0}Account/Join?returnurl={1}", teamMemberRequest.SiteBaseUrl, teamMemberRequest.ActivationCode);
                emailBody = emailBody.Replace("@teamName", teamMemberRequest.Team.Name);
                emailBody = emailBody.Replace("@joinUrl", joinLink);
                emailBody = emailBody.Replace("@inviter", teamMemberRequest.CreatedBy.Name);
                email.Body = emailBody;
                email.Subject = emailSubject;
                await Send(email);
                //}

            }
        }

        private async Task Send(Email email)
        {

            try
            {
                //var mail = new MailMessage();
                //string smtpServerName = configuration.Get<string>("TeamBins:Email:SmtpServer");
                //var smtp = new SmtpClient(smtpServerName);
                //smtp.Port = configuration.Get<int>("TeamBins:Email:Port");
                //var from = configuration.Get<string>("TeamBins:Email:FromEmailAddress");
                //MailAddress fromAddress = new MailAddress(from, "TeamBins");
                //mail.From = fromAddress;
                //foreach (var address in email.ToAddress)
                //{
                //    mail.To.Add(address);
                //}
                //var bcc = configuration.Get<string>("TeamBins:Email:BccEmailAddress");
                //if (!String.IsNullOrEmpty(bcc))
                //{
                //    var addressBCC = new MailAddress(bcc);
                //    mail.Bcc.Add(addressBCC);
                //}


                //string senderUserName = configuration.Get<string>("TeamBins:Email:UserName");
                //string senderPassword = configuration.Get<string>("TeamBins:Email:Password");

                //NetworkCredential basicCredential = new NetworkCredential(senderUserName, senderPassword);

                //mail.Subject = email.Subject;
                //mail.Body = email.Body;
                //mail.IsBodyHtml = true;
                //smtp.UseDefaultCredentials = false;
                //smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                //smtp.Credentials = basicCredential;
                //smtp.EnableSsl = true;

                //await smtp.SendMailAsync(mail);
            }
            catch (Exception ex)
            {

            }

        }

    }




}