using System.Collections.Generic;

namespace EmailSender
{
	public class Config
	{
		public ClientType ClientType { get; set; }
		public string Id { get; set; }
		public string Pwd { get; set; }
		public EmailAddress FromEmail { get; set; }

		public List<EmailAddress> ToEmails { get; set; }

		public string Subject { get; set; }
		public string HtmlBody { get; set; }
		public string TextBody { get; set; }
	}
}
