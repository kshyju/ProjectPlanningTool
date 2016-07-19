//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace TeamBins6.Infrastrucutre.Extensions
//{
//    //Not really an extension method on string ! I still prefer to just call it like the ext method style ;)
//    public static class EmailExtensions
//    {
//        public static string ToGravatarHash(string email)
//        {
//            var encoder = new System.Text.UTF8Encoding();
//            var md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
//            var hashedBytes = md5.ComputeHash(encoder.GetBytes(email.ToLower()));
//            var sb = new System.Text.StringBuilder(hashedBytes.Length * 2);

//            for (var i = 0; i < hashedBytes.Length; i++)
//                sb.Append(hashedBytes[i].ToString("X2"));

//            return sb.ToString().ToLower();
//        }

//        public static string GetAvatarUrl(string avatar, int size = 0)
//        {
//            var imageUrl = "http://www.gravatar.com/avatar/" + avatar;
//            if (size > 0)
//                imageUrl += "?s=" + size;

//            return imageUrl;
//        }
//        public static string ToGravatarUrl(this string email, int size = 40)
//        {

//            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(email.Trim()))
//                throw new ArgumentException("The email is empty.", "email");

//            var imageUrl = "http://www.gravatar.com/avatar.php?";

//            var sb = ToGravatarHash(email);

//            imageUrl += "gravatar_id=" + sb;
//            if (size > 0)
//                imageUrl += "?s=" + size;

//            return imageUrl;

//        }

//    }
//}
