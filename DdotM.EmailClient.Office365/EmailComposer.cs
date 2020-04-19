using MimeKit;

namespace DdotM.EmailClient.Office365
{
    public static class EmailComposer
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

        public static void AddCcAddress(MimeMessage message, string name, string address)
        {
            var from = new MailboxAddress(name, address);
            message.Cc.Add(from);
        }

        public static void AddBccAddress(MimeMessage message, string name, string address)
        {
            var from = new MailboxAddress(name, address);
            message.Bcc.Add(from);
        }
    }
}
