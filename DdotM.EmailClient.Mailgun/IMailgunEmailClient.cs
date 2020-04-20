using RestSharp;
using System.Threading.Tasks;

namespace DdotM.EmailClient.Mailgun
{
    public interface IMailgunEmailClient
    {
        Task<IRestResponse> SendAsync(MailgunMessage msg);
    }
}
