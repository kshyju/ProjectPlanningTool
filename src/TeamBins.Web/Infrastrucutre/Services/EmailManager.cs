using System;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
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

        // to do : We need to move anything other than Sending emaisl to it's own classes like we did for CommentManager

        Task Send(Email email);
    }


    public class EmailManager : IEmailManager
    {
        private readonly IEmailRepository _emailRepository;
        private readonly ITeamRepository _teamRepository;
        readonly AppSettings _settings;

        public EmailManager(IEmailRepository emailRepository, IOptions<AppSettings> settings,
            ITeamRepository teamRepository)
        {
            this._teamRepository = teamRepository;
            this._emailRepository = emailRepository;
            this._settings = settings.Value;
        }

        public async Task SendAccountCreatedEmail(UserDto newUser)
        {
            try
            {
                var emailTemplate = await _emailRepository.GetEmailTemplate("NewAccount");
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
                var subscibers = await _teamRepository.GetSubscribers(teamId, "NewIssue");
                if (subscibers.Any())
                {
                    var emailTemplate = await _emailRepository.GetEmailTemplate("NewIssue");
                    if (emailTemplate != null)
                    {
                        var emailSubject = emailTemplate.Subject;
                        var emailBody = emailTemplate.EmailBody;
                        var email = new Email();


                        foreach (var subsciber in subscibers)
                        {
                            email.ToAddress.Add(subsciber.EmailAddress);
                        }
                        var team = _teamRepository.GetTeam(teamId);

                        var issueUrl = this._settings.SiteUrl + "/issues/" + issue.Id;
                        var issueLink = string.Format("<a href='{0}'>" + "#{1} {2}</a>", issueUrl, issue.Id, issue.Title);

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
                var emailTemplate = await _emailRepository.GetEmailTemplate("JoinMyTeam");
                if (emailTemplate != null)
                {
                    var emailSubject = emailTemplate.Subject;
                    var emailBody = emailTemplate.EmailBody;
                    var email = new Email();
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
            await Task.Run(() =>
            {
                SendEmail(email);
            });

        }
        

        private  void SendEmail(Email email)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("TeamBins", this._settings.Email.FromEmailAddress));
                foreach (var address in email.ToAddress)
                {
                    message.To.Add(new MailboxAddress(address, address));
                }

                if(!String.IsNullOrEmpty(this._settings.Email.BccEmailAddress))
                    message.Bcc.Add(new MailboxAddress(this._settings.Email.BccEmailAddress, this._settings.Email.BccEmailAddress));

                message.Subject = email.Subject;
                var builder = new BodyBuilder { HtmlBody = email.Body };

                message.Body = builder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    client.Connect(this._settings.Email.SmtpServer, this._settings.Email.Port, false);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(this._settings.Email.UserName, this._settings.Email.Password);

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