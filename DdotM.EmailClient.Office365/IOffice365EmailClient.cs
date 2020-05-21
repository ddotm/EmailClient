using System.Threading.Tasks;

namespace DdotM.EmailClient.Office365
{
    public interface IOffice365EmailClient
    {
        Task SendAsync(EmailMessage emailMessage);
    }
}
