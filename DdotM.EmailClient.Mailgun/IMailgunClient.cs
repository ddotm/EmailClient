using RestSharp;
using System.Threading.Tasks;

namespace DdotM.EmailClient.Mailgun
{
    public interface IMailgunClient
    {
        Task<IRestResponse> SendAsync(MailgunMessage msg);
    }
}
