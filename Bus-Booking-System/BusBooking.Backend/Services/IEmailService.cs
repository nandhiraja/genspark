using System.Threading.Tasks;

namespace BusBooking.Backend.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body, bool saveToMockInbox = true);
    }
}