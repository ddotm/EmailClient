using DdotM.EmailClient.Common;
using RestSharp;
using System;
using System.Collections.Generic;

namespace DdotM.EmailClient.Mailgun
{
    /// <summary>
    /// Mailgun email message
    /// </summary>
    public class MailgunMessage : EmailMessage
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public MailgunMessage()
        {
            Tags = new List<string>();
        }

        /// <summary>
        /// Message delivery time.
        /// Messages are not guaranteed to arrive at exactly the requested time.
        /// Messages can be scheduled for a maximum of 3 days in the future.
        /// </summary>
        public DateTime? DeliveryTime { get; set; }
        
        /// <summary>
        /// Mailgun message tags
        /// </summary>
        public List<string> Tags { get; }
        
        /// <summary>
        /// Controls message tracking. Default is false.
        /// </summary>
        public bool Tracking { get; set; } = false;

        /// <summary>
        /// Once the message send is attempted, Response will contain the Mailgun HTTP response data
        /// </summary>
        public IRestResponse Response { get; set; }
    }
}
