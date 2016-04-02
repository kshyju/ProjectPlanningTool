using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;

namespace TeamBins.Common.ViewModels
{
    public class KeyValueItem
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public string Code { set; get; }
    }
    public class Email
    {
        public string FromAddress { set; get; }
        public List<string> ToAddress { set; get; }
        public string Subject { set; get; }
        public string Body { set; get; }

        public Email()
        {
            ToAddress = new List<string>();
        }
        public void Send()
        {

            try
            {
                //MailMessage mail = new MailMessage();
                //string smtpServerName = ConfigurationManager.AppSettings["smtpServer"] as string;
                //SmtpClient smtp = new SmtpClient(smtpServerName);
                //smtp.Port = Convert.ToInt32(ConfigurationManager.AppSettings["smtpPort"] as string);
                //FromAddress = ConfigurationManager.AppSettings["emailIdFrom"] as string;
                //MailAddress fromAddress = new MailAddress(FromAddress, "TeamBins");
                //mail.From = fromAddress;
                //foreach (var address in ToAddress)
                //{
                //    mail.To.Add(address);
                //}
                //MailAddress addressBCC = new MailAddress(ConfigurationManager.AppSettings["bccTrackingEmail"] as string);
                //mail.Bcc.Add(addressBCC);

                //string senderUserName = ConfigurationManager.AppSettings["senderUserName"] as string;
                //string senderPassword = ConfigurationManager.AppSettings["senderPassword"] as string;

               // NetworkCredential basicCredential = new NetworkCredential(senderUserName, senderPassword);

               // mail.Subject = Subject;
               // mail.Body = Body;
               // mail.IsBodyHtml = true;
               // smtp.UseDefaultCredentials = false;
               // smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
               //// smtp.Credentials = basicCredential;
               // smtp.EnableSsl = true;

                // smtp.Send(mail);
            }
            catch (Exception ex)
            {
                //var log = new Logger("Email");
                //log.Error(ex);
            }

        }
    }
}