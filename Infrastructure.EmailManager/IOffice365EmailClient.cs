using DdotM.EmailClient.Common;
using System.Threading.Tasks;

namespace Infrastructure.EmailManager
{
    public interface IOffice365EmailClient
    {
        Task SendAsync(EmailMessageConfig emailMessageConfig);
    }
}
