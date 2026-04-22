using System;
using System.Threading.Tasks;

namespace BusBooking.Backend.Services
{
    public class EmailService : IEmailService
    {
        public Task SendEmailAsync(string to, string subject, string body)
        {
            Console.WriteLine($"[EMAIL SENT to {to}] Subject: {subject}");
            return Task.CompletedTask;
        }
    }
}
