using DdotM.EmailClient.Common;
using System;
using System.Collections.Generic;

namespace DdotM.EmailClient.Mailgun
{
    public class MailgunMessage : EmailMessage
    {
        public MailgunMessage()
        {
            Tags = new List<string>();
        }

        // Set message delivery time - format "Fri, 14 Oct 2011 23:10:10 -0000"
        public DateTime? DeliveryTime { get; set; }
        public List<string> Tags { get; set; }
        public bool Tracking { get; set; } = false;
    }
}
