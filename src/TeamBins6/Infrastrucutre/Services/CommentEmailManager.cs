using System.Threading.Tasks;
using TeamBins.Common;
using TeamBins.Common.Infrastructure;
using TeamBins.Common.Infrastructure.Enums.TeamBins.Helpers.Enums;

namespace TeamBins.Services
{
    public interface ICommentEmailManager
    {
        Task SendNewCommentEmail(CommentVM comment, int teamId);
    }
    //public class CommentEmailManager : ICommentEmailManager
    //{
    //    readonly IUserRepository userRepository;
    //    readonly IEmailTemplateRepository emailTemplateRepository;
    //    public CommentEmailManager(IEmailTemplateRepository emailTemplateRepository,IUserRepository userRepository)
    //    {
    //        this.emailTemplateRepository = emailTemplateRepository;
    //        this.userRepository = userRepository;
    //    }

    //    public async Task SendNewCommentEmail(CommentVM comment,int teamId)
    //    {
    //        var subscribers = await userRepository.GetSubscribers(teamId, NotificationTypeCode.NewComment);

    //        var emailTemplate =  emailTemplateRepository.GetEmailTemplate("NewComment");
    //        if (emailTemplate != null)
    //        {
    //            string emailSubject = emailTemplate.Subject;
    //            string emailBody = emailTemplate.EmailBody;
    //            Email email = new Email();
               
              
    //            string issueUrl = UrlBuilderHelper.SiteBaseUrl + "issues/" + comment.IssueId;
    //            var issueLink = "<a href='" + issueUrl + "'>" + "# " + comment.IssueId + " " + comment.IssueId + "</a>";
    //            emailBody = emailBody.Replace("@author", comment.Author.Name);
    //            emailBody = emailBody.Replace("@link", issueLink);
    //            emailBody = emailBody.Replace("@comment", comment.CommentBody);
    //            emailBody = emailBody.Replace("@issueId", comment.IssueId.ToString());
    //            email.Body = emailBody;
    //            emailSubject = emailSubject.Replace("@issueId", comment.IssueId.ToString());
    //            email.Subject = emailSubject;

    //            foreach (var subscriber in subscribers)
    //            {
    //                email.ToAddress.Add(subscriber.EmailAddress);
    //            }

    //            email.Send();
    //        }
    //    }

    //}
}