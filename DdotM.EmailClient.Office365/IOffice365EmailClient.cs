using System.Threading.Tasks;
using DdotM.EmailClient.Common;

namespace DdotM.EmailClient.Office365
{
    public interface IOffice365EmailClient
    {
        Task SendAsync(EmailMessageConfig emailMessageConfig);
    }
}
