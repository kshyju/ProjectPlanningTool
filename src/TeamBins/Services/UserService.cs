using System;
using TeamBins.DataAccess;
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

        public void SendJoinMyTeamEmail(TeamMemberRequest teamMemberRequest)
        {    
            var emailTemplate = repo.GetEmailTemplate("JoinMyTeam");
            if (emailTemplate != null)
            {              
                string emailBody = emailTemplate.EmailBody;
                Email email = new Email();
                email.ToAddress.Add(teamMemberRequest.EmailAddress);

                string joinLink = String.Format("{0}/Account/Join?returnurl={1}", SiteBaseURL, teamMemberRequest.ActivationCode);
                emailBody = emailBody.Replace("@teamName", teamMemberRequest.Team.Name);
                emailBody = emailBody.Replace("@link", joinLink);
                emailBody=emailBody.Replace("@inviter", teamMemberRequest.CreatedBy.FirstName);

                email.Send();
            }


        }
        public static string GetImageSource(string email,int size=0)
        {


            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(email.Trim()))
                throw new ArgumentException("The email is empty.", "email");

            var imageUrl = "http://www.gravatar.com/avatar.php?";
            var encoder = new System.Text.UTF8Encoding();
            var md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            var hashedBytes = md5.ComputeHash(encoder.GetBytes(email.ToLower()));
            var sb = new System.Text.StringBuilder(hashedBytes.Length * 2);

            for (var i = 0; i < hashedBytes.Length; i++)
                sb.Append(hashedBytes[i].ToString("X2"));

            imageUrl += "gravatar_id=" + sb.ToString().ToLower();
           // imageUrl += "&rating=PG";
            if (size>0)
                imageUrl += "?s="+size;           

            return imageUrl;

        }
    
        

    }
}
