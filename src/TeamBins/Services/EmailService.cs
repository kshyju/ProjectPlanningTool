using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace TechiesWeb.TeamBins.Services
{
    public class EmailService
    {
        public bool SendEmail(string toAddress, string messageBody, string subject)
        {
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

            mail.From = new MailAddress("myemail@gmail.com");
            mail.To.Add("recepient@gmail.com");
            mail.Subject = "Password recovery";
            mail.Body = "Recovering the password";

            SmtpServer.Send(mail);
            return true;

        }
    }
}