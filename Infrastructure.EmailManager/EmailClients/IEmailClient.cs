using System;
using System.Threading.Tasks;

namespace Infrastructure.EmailManager.EmailClients
{
    public interface IEmailClient: IAsyncDisposable
    {
        Task SendAsync(EmailMessageConfig emailMessageConfig);
    }
}
