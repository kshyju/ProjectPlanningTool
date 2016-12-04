using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using TeamBins.Common;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;
using Newtonsoft.Json;
using TeamBins.DataAccessCore;
using TeamBins.Common.ViewModels;
using TeamBins.DataAccess;
using TeamBins.Infrastrucutre;

namespace TeamBins.Services
{
    public interface IEmailManager
    {
        Task SendTeamMemberInvitationEmail(AddTeamMemberRequestVM teamMemberRequest);

        Task SendAccountCreatedEmail(UserDto newUser);

        Task SendIssueCreatedEmail(IssueDetailVM issue, int teamId);

        Task Send(Email email);
    }


    public class EmailManager : IEmailManager
    {
        private IEmailRepository emailRepository;
        private ITeamRepository teamRepository;

        readonly AppSettings settings;

        public EmailManager(IEmailRepository emailRepository, IOptions<AppSettings> settings,
            ITeamRepository teamRepository)
        {
            this.teamRepository = teamRepository;
            this.emailRepository = emailRepository;

            this.settings = settings.Value;
        }

        public async Task SendAccountCreatedEmail(UserDto newUser)
        {
            try
            {
                var emailTemplate = await emailRepository.GetEmailTemplate("NewAccount");
                if (emailTemplate != null)
                {
                    var emailSubject = emailTemplate.Subject;
                    var emailBody = emailTemplate.EmailBody;
                    var email = new Email();
                    email.ToAddress.Add(newUser.EmailAddress);

                    emailBody = emailBody.Replace("@userName", newUser.Name);
                    email.Body = emailBody;
                    email.Subject = emailSubject;
                    await Send(email);
                }
            }
            catch (Exception)
            {
                // Silently fail. We will log this. But we do not want to show an error to user because of this
            }

        }

        public async Task SendIssueCreatedEmail(IssueDetailVM issue, int teamId)
        {
            try
            {
                var subscibers = await teamRepository.GetSubscribers(teamId, "NewIssue");
                if (subscibers.Any())
                {
                    var emailTemplate = await emailRepository.GetEmailTemplate("NewIssue");
                    if (emailTemplate != null)
                    {
                        var emailSubject = emailTemplate.Subject;
                        var emailBody = emailTemplate.EmailBody;
                        var email = new Email();


                        foreach (var subsciber in subscibers)
                        {
                            email.ToAddress.Add(subsciber.EmailAddress);
                        }
                        var team = teamRepository.GetTeam(teamId);

                        var issueUrl = this.settings.SiteUrl + "/issues/" + issue.Id;
                        var issueLink = string.Format("<a href='{0}'>" + "#{0} {0}</a>", issueUrl, issue.Id, issue.Title);

                        emailBody = emailBody.Replace("@issueAuthor", issue.Author.Name);
                        emailBody = emailBody.Replace("@teamName", team.Name);
                        emailBody = emailBody.Replace("@issueLink", issueLink);
                        emailSubject = emailSubject.Replace("@teamName", team.Name);

                        email.Body = emailBody;
                        email.Subject = emailSubject;
                        await Send(email);
                    }
                }
            }
            catch (Exception)
            {
                // Silently fail. We will log this. But we do not want to show an error to user because of this
            }

        }

        public async Task SendTeamMemberInvitationEmail(AddTeamMemberRequestVM teamMemberRequest)
        {
            try
            {
                var emailTemplate = await emailRepository.GetEmailTemplate("JoinMyTeam");
                if (emailTemplate != null)
                {
                    var emailSubject = emailTemplate.Subject;
                    var emailBody = emailTemplate.EmailBody;
                    Email email = new Email();
                    email.ToAddress.Add(teamMemberRequest.EmailAddress);

                    var joinLink = String.Format("{0}Account/Join?returnurl={1}", teamMemberRequest.SiteBaseUrl, teamMemberRequest.ActivationCode);
                    emailBody = emailBody.Replace("@teamName", teamMemberRequest.Team.Name);
                    emailBody = emailBody.Replace("@joinUrl", joinLink);
                    emailBody = emailBody.Replace("@inviter", teamMemberRequest.CreatedBy.Name);
                    email.Body = emailBody;
                    email.Subject = emailSubject;
                    await Send(email);
                }
            }
            catch (Exception)
            {
                // Silently fail. We will log this. But we do not want to show an error to user because of this
            }

        }

        public async Task Send(Email email)
        {
            await SendEmail(email);
            //await Task.Run(() =>
            //{
            //    SendEmail(email);
            //});

        }

        private async Task SendEmail(Email email)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("TeamBins", this.settings.Email.FromEmailAddress));
                foreach (var address in email.ToAddress)
                {
                    message.To.Add(new MailboxAddress(address, address));
                }

                if(!String.IsNullOrEmpty(this.settings.Email.BccEmailAddress))
                    message.Bcc.Add(new MailboxAddress(this.settings.Email.BccEmailAddress, this.settings.Email.BccEmailAddress));

                message.Subject = email.Subject;
                var builder = new BodyBuilder { HtmlBody = email.Body };

                message.Body = builder.ToMessageBody();
                using (var client = new SmtpClient())
                {
                    // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    client.Connect(this.settings.Email.SmtpServer, 587, false);

                    // Note: since we don't have an OAuth2 token, disable
                    // the XOAUTH2 authentication mechanism.
                    client.AuthenticationMechanisms.Remove("XOAUTH2");

                    // Note: only needed if the SMTP server requires authentication
                    await client.AuthenticateAsync(this.settings.Email.UserName, this.settings.Email.Password);
                    //client.Authenticate("teambinsprojects@gmail.com", "OpenSource123");

                    client.Send(message);
                    client.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }




}