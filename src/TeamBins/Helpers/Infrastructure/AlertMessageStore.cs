using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace TechiesWeb.TeamBins.Infrastructure
{
    public class AlertMessageStore
    { 
        public AlertMessageStore()
        {
            Messages = new List<KeyValuePair<string, string>>();
        }

        public ICollection<KeyValuePair<string, string>> Messages { get; private set; }

        public void AddMessage(string messageType, string alertMessage)
        {
            Messages.Add(new KeyValuePair<string, string>(messageType, alertMessage));
        }
    }
}