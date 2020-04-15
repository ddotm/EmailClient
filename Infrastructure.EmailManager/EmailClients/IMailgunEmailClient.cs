using RestSharp;
using System.Threading.Tasks;

namespace Infrastructure.EmailManager.EmailClients
{
    public interface IMailgunEmailClient
    {
        Task<IRestResponse> SendAsync(EmailMessageConfig msg);
    }
}
