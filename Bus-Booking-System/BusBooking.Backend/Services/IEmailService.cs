using System.Threading.Tasks;

namespace BusBooking.Backend.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}
