using RestSharp;
using System;
using System.Collections.Generic;

namespace DdotM.EmailClient.Mailgun
{
    /// <summary>
    /// Mailgun email message
    /// </summary>
    public class MailgunMessage
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public MailgunMessage()
        {
            From = new Recipient();
            ToEmails = new List<Recipient>();
            CcEmails = new List<Recipient>();
            BccEmails = new List<Recipient>();
            Tags = new List<string>();
        }

        /// <summary>
        /// Email sender
        /// </summary>
        public Recipient From { get; }

        /// <summary>
        /// List of the To email recipients
        /// </summary>
        public List<Recipient> ToEmails { get; }

        /// <summary>
        /// List of the Cc email recipients
        /// </summary>
        public List<Recipient> CcEmails { get; }

        /// <summary>
        /// List of the Bcc email recipients
        /// </summary>
        public List<Recipient> BccEmails { get; }

        /// <summary>
        /// Email subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Email contents in text format
        /// </summary>
        public string TextBody { get; set; }

        /// <summary>
        /// Email contents in HTML format
        /// </summary>
        public string HtmlBody { get; set; }

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
