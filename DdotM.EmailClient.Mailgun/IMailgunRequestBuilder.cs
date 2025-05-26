namespace DdotM.EmailClient.Mailgun;

/// <summary>
/// Defines the contract for building a Mailgun HTTP request content object
/// from a message and configuration.
/// </summary>
public interface IMailgunRequestBuilder
{
    /// <summary>
    /// Builds the appropriate HTTP content for a Mailgun API request,
    /// using the provided message and configuration.
    /// </summary>
    /// <param name="msg">The <see cref="MailgunMessage"/> containing message details.</param>
    /// <returns>
    /// An <see cref="HttpContent"/> instance suitable for sending to Mailgun's API.
    /// </returns>
    HttpContent Build(MailgunMessage msg);
}