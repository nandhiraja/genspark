using BusBooking.Backend.Models;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace BusBooking.Backend.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;

        public EmailService(IConfiguration config, ApplicationDbContext context)
        {
            _config = config;
            _context = context;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body, bool saveToMockInbox = true)
        {
            var settings = _config.GetSection("EmailSettings");
            var smtpHost    = settings["SmtpHost"] ?? "smtp.gmail.com";
            var smtpPort    = int.Parse(settings["SmtpPort"] ?? "587");
            var senderEmail = settings["SenderEmail"] ?? "";
            var senderName  = settings["SenderName"] ?? "RideBus";
            var password    = settings["Password"] ?? "";

            if (saveToMockInbox)
            {
                await SaveMockEmailAsync(
                    senderEmail: senderEmail,
                    toEmail: toEmail,
                    subject: subject,
                    body: body
                );
            }

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                EnableSsl   = true,
                Credentials = new NetworkCredential(senderEmail, password)
            };

            var mail = new MailMessage
            {
                From       = new MailAddress(senderEmail, senderName),
                Subject    = subject,
                Body       = body,
                IsBodyHtml = false
            };
            mail.To.Add(toEmail);

            await client.SendMailAsync(mail);
        }

        private async Task SaveMockEmailAsync(string senderEmail, string toEmail, string subject, string body)
        {
            var from = string.IsNullOrWhiteSpace(senderEmail) ? "no-reply@gobus.local" : senderEmail.Trim().ToLowerInvariant();
            var to = toEmail.Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(to)) return;

            await _context.MockMailMessages.AddAsync(new MockMailMessage
            {
                FromEmail = from,
                ToEmail = to,
                Subject = string.IsNullOrWhiteSpace(subject) ? "(No subject)" : subject.Trim(),
                Body = string.IsNullOrWhiteSpace(body) ? "(Empty message)" : body.Trim()
            });
            await _context.SaveChangesAsync();
        }
    }
}