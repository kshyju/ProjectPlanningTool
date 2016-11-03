using System;
using System.Collections.Generic;
using System.Linq;

namespace TeamBins.Infrastrucutre
{
    // [Serializable]

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
