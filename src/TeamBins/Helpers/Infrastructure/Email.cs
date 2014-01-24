using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using TechiesWeb.TeamBibs.Helpers.Logging;

namespace TechiesWeb.TeamBins.Infrastructure
{
    public class Email
    {
        public string FromAddress { set; get; }
        public List<string> ToAddress { set; get; }
        public string Subject { set; get; }
        public string Body { set; get; }

        public void Send()
        {
            try
            {
                MailMessage mail = new MailMessage();
                string smtpServerName = ConfigurationManager.AppSettings["smtpServer"] as string;
                SmtpClient smtp = new SmtpClient(smtpServerName);
                smtp.Port = Convert.ToInt32(ConfigurationManager.AppSettings["smtpPort"] as string);
                MailAddress fromAddress = new MailAddress(FromAddress, "TeamBins");
                foreach (var address in ToAddress)
                {
                    mail.To.Add(address);
                }
                MailAddress addressBCC = new MailAddress(ConfigurationManager.AppSettings["bccTrackingEmail"] as string);
                mail.Bcc.Add(addressBCC);

                string senderUserName = ConfigurationManager.AppSettings["senderUserName"] as string;
                string senderPass = ConfigurationManager.AppSettings["senderPassword"] as string;

                NetworkCredential basicCredential = new NetworkCredential(senderUserName, senderPass);

                mail.Subject = Subject;
                mail.Body = Body;
                mail.IsBodyHtml = true;
                smtp.UseDefaultCredentials = false;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Credentials = basicCredential;
                smtp.EnableSsl = true;

                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                var log = new Logger("Email");
                log.Error(ex);
            }

        }
    }
}
