using Planner.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TeamBins.DataAccess;

namespace TeamBins.Services
{

    public class UserService
    {
        IRepositary repo;
        public UserService(IRepositary repositary)
        {
            repo = repositary;
        }
        public bool SaveDefaultIssueSettings(int userId,int defaultProjectId)
        {
            var user = repo.GetUser(userId);
            if (user != null)
            {
                user.DefaultProjectID = defaultProjectId;
                var result = repo.SaveUser(user);
                return result.Status;                
            }
            return false;
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
