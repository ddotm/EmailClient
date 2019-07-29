using MimeKit;

namespace EmailSender
{
	public class EmailComposer
	{
		public static void AddFromAddress(MimeMessage message, string name, string address)
		{
			var from = new MailboxAddress(name, address);
			message.From.Add(from);
		}

		public static void AddToAddress(MimeMessage message, string name, string address)
		{
			var from = new MailboxAddress(name, address);
			message.To.Add(from);
		}
	}
}
