using System.Collections.Generic;
using Microsoft.Extensions.Configuration;


namespace TeamBins6.Common
{
    public class Email
    {
        private IConfiguration configuration;
        public Email(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string FromAddress { set; get; }
        public List<string> ToAddress { set; get; }
        public string Subject { set; get; }
        public string Body { set; get; }

        public Email()
        {
            ToAddress = new List<string>();
        }
        //public void Send()
        //{

        //    try
        //    {
        //        MailMessage mail = new MailMessage();
        //        string smtpServerName = configuration.Get<string>("TeamBins:Email:UserName");
        //        SmtpClient smtp = new SmtpClient(smtpServerName);
        //        smtp.Port = configuration.Get<int>("TeamBins:Email:Port");
        //        FromAddress = configuration.Get<string>("TeamBins:Email:FromEmailAddress");
        //        MailAddress fromAddress = new MailAddress(FromAddress, "TeamBins");
        //        mail.From = fromAddress;
        //        foreach (var address in ToAddress)
        //        {
        //            mail.To.Add(address);
        //        }
        //        var bcc = configuration.Get<string>("TeamBins:Email:BccEmailAddress");
        //        if (!String.IsNullOrEmpty(bcc))
        //        {
        //            var addressBCC = new MailAddress(bcc);
        //            mail.Bcc.Add(addressBCC);
        //        }


        //        string senderUserName = configuration.Get<string>("TeamBins:Email:UserName");
        //        string senderPassword = configuration.Get<string>("TeamBins:Email:Password");

        //        NetworkCredential basicCredential = new NetworkCredential(senderUserName, senderPassword);

        //        mail.Subject = Subject;
        //        mail.Body = Body;
        //        mail.IsBodyHtml = true;
        //        smtp.UseDefaultCredentials = false;
        //        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
        //        smtp.Credentials = basicCredential;
        //        smtp.EnableSsl = true;

        //        // smtp.Send(mail);
        //    }
        //    catch (Exception ex)
        //    {

        //    }

        //}
    }
}
