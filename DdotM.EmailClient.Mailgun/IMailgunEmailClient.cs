using DdotM.EmailClient.Common;
using RestSharp;
using System.Threading.Tasks;

namespace DdotM.EmailClient.Mailgun
{
    public interface IMailgunEmailClient
    {
        Task<IRestResponse> SendAsync(EmailMessageConfig msg);
    }
}
