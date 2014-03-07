using System;
using TeamBins.DataAccess;
using TechiesWeb.TeamBibs.Helpers.Logging;
using TechiesWeb.TeamBins.Infrastructure;
namespace TeamBins.Services
{
    public class UserService
    {
        public string SiteBaseURL { set; get; }
        public UserService()
        {          
        }
        public UserService(string siteBaseUrl)
        {
            SiteBaseURL = siteBaseUrl;
        }
        IRepositary repo;
        public UserService(IRepositary repositary)
        {
            repo = repositary;
        }
        public UserService(IRepositary repositary, string siteBaseUrl)
        {
            repo = repositary;
            SiteBaseURL = siteBaseUrl;
        }
       
        public bool SaveDefaultProjectForTeam(int userId,int teamId, int defaultProjectId)
        {
            var teamMember = repo.GetTeamMember(userId, teamId);
            if (teamMember != null)
            {
                teamMember.DefaultProjectID = defaultProjectId;
                var res = repo.SaveTeamMember(teamMember);
                return true;
            }          
            return false;
        }
        public int GetDefaultProjectForCurrentTeam(int userId, int teamId)
        {
            var teamMember = repo.GetTeamMember(userId, teamId);
            if (teamMember != null)
            {
                if (teamMember.DefaultProjectID.HasValue)
                   return teamMember.DefaultProjectID.Value;
            }
            return 0;
        }
        public void SaveActivityForNewUserJoinedTeam(TeamMemberRequest teamMemberRequest, User user,int currentUserId,int teamId)
        {
            var activity = new Activity { CreatedByID = currentUserId, TeamID = teamId };
            activity.ObjectID = user.ID;
            activity.ObjectType = "User";
            activity.ActivityDesc = "Joined team " + teamMemberRequest.Team.Name;
            activity.NewState = user.FirstName;

            var result = repo.SaveActivity(activity);
            if (!result.Status)
            {
                var log = new Logger("Email");
                log.Error(result);
            }
        }


        public void SendNewAccountCreatedEmail(User user)
        {
            var emailTemplate = repo.GetEmailTemplate("NewAccount");
            if (emailTemplate != null)
            {
                string emailSubject = emailTemplate.EmailSubject;
                string emailBody = emailTemplate.EmailBody;
                Email email = new Email();
                email.ToAddress.Add(user.EmailAddress);
              
                emailBody = emailBody.Replace("@userName", user.FirstName);
                email.Body = emailBody;
                email.Subject = emailSubject;
                email.Send();
            }
        }
        public void SendResetPasswordEmail(PasswordResetRequest request)
        {
            var emailTemplate = repo.GetEmailTemplate("ResetPassword");
            if (emailTemplate != null)
            {                

                string emailSubject = emailTemplate.EmailSubject;
                string emailBody = emailTemplate.EmailBody;
                Email email = new Email();
                email.ToAddress.Add(request.User.EmailAddress);

                string joinLink = String.Format("{0}Account/resetpassword/{1}", SiteBaseURL, request.ActivationCode);
                emailBody = emailBody.Replace("@resetLink", joinLink);              
                email.Body = emailBody;
                email.Subject = emailSubject;
                email.Send();
            }


        }
        public void SendJoinMyTeamEmail(TeamMemberRequest teamMemberRequest)
        {    
            var emailTemplate = repo.GetEmailTemplate("JoinMyTeam");
            if (emailTemplate != null)
            {
                teamMemberRequest = repo.GetTeamMemberRequest(teamMemberRequest.ActivationCode);

                string emailSubject = emailTemplate.EmailSubject;
                string emailBody = emailTemplate.EmailBody;
                Email email = new Email();
                email.ToAddress.Add(teamMemberRequest.EmailAddress);

                
                string joinLink = String.Format("{0}Account/Join?returnurl={1}", SiteBaseURL, teamMemberRequest.ActivationCode);
                emailBody = emailBody.Replace("@teamName", teamMemberRequest.Team.Name);
                emailBody = emailBody.Replace("@joinUrl", joinLink);
                emailBody=emailBody.Replace("@inviter", teamMemberRequest.CreatedBy.FirstName);
                email.Body = emailBody;
                email.Subject=emailSubject;
                email.Send();
            }


        }
        public static string GetAvatarUrl(string avatar, int size = 0)
        {
            var imageUrl = "http://www.gravatar.com/avatar/"+avatar;
            if (size > 0)
                imageUrl += "?s=" + size;

            return imageUrl;
        }
        public static string GetImageSource(string email,int size=0)
        {

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(email.Trim()))
                throw new ArgumentException("The email is empty.", "email");

            var imageUrl = "http://www.gravatar.com/avatar.php?";

            var sb = GetGravatarHash(email);

            imageUrl += "gravatar_id=" + sb;          
            if (size>0)
                imageUrl += "?s="+size;           

            return imageUrl;

        }

        public static string GetGravatarHash(string email)
        {
            var encoder = new System.Text.UTF8Encoding();
            var md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            var hashedBytes = md5.ComputeHash(encoder.GetBytes(email.ToLower()));
            var sb = new System.Text.StringBuilder(hashedBytes.Length * 2);

            for (var i = 0; i < hashedBytes.Length; i++)
                sb.Append(hashedBytes[i].ToString("X2"));

            return sb.ToString().ToLower();
        }
    
        

    }
}
