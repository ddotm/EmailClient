using RestSharp;
using System.Threading.Tasks;

namespace DdotM.EmailClient.Mailgun
{
    /// <summary>
    /// Mailgun client
    /// </summary>
    public interface IMailgunClient
    {
        /// <summary>
        /// Sends an email message through Mailgun
        /// </summary>
        /// <param name="msg">Message to send</param>
        /// <returns></returns>
        Task<IRestResponse> SendAsync(MailgunMessage msg);
    }
}
