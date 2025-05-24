namespace DdotM.EmailClient.Mailgun;

/// <summary>
/// Defines a contract for sending email messages via the Mailgun API.
/// </summary>
public interface IMailgunClient
{
    /// <summary>
    /// Sends an email message through Mailgun.
    /// </summary>
    /// <param name="msg">The <see cref="MailgunMessage"/> to send.</param>
    /// <returns>
    /// A task representing the asynchronous send operation. 
    /// The result contains the <see cref="MailgunMessage"/> populated with the HTTP response.
    /// </returns>
    Task<MailgunMessage> SendAsync(MailgunMessage msg);
}