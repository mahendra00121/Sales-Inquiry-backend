using System.Threading.Tasks;

namespace PolyTrack.API.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}
