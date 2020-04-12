using System.Threading.Tasks;

namespace Infrastructure.EmailManager.EmailClients
{
    public interface IEmailClient
    {
        Task SendAsync(EmailMessageConfig emailMessageConfig);
    }
}
