using BusBooking.Backend.Models;
using BusBooking.Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BusBooking.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MailController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public MailController(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // GET /api/mail/inbox
        [HttpGet("inbox")]
        public async Task<IActionResult> Inbox([FromQuery] int limit = 40)
        {
            var email = await GetCurrentUserEmailAsync();
            if (email == null) return Unauthorized();

            limit = Math.Clamp(limit, 1, 200);
            var lower = email.ToLowerInvariant();

            var rows = await _context.MockMailMessages
                .AsNoTracking()
                .Where(m => m.ToEmail.ToLower() == lower || m.FromEmail.ToLower() == lower)
                .OrderByDescending(m => m.CreatedAt)
                .Take(limit)
                .Select(m => new
                {
                    m.Id,
                    m.FromEmail,
                    m.ToEmail,
                    m.Subject,
                    m.Body,
                    m.IsRead,
                    m.CreatedAt,
                    m.ReadAt,
                    m.ParentMessageId,
                    Direction = m.ToEmail.ToLower() == lower ? "INBOX" : "SENT"
                })
                .ToListAsync();

            var unreadCount = await _context.MockMailMessages
                .CountAsync(m => m.ToEmail.ToLower() == lower && !m.IsRead);

            return Ok(new { unreadCount, items = rows });
        }

        // PUT /api/mail/{id}/read
        [HttpPut("{id:guid}/read")]
        public async Task<IActionResult> MarkRead(Guid id)
        {
            var email = await GetCurrentUserEmailAsync();
            if (email == null) return Unauthorized();
            var lower = email.ToLowerInvariant();

            var msg = await _context.MockMailMessages.FirstOrDefaultAsync(m => m.Id == id);
            if (msg == null) return NotFound(new { message = "Mail not found." });
            if (!string.Equals(msg.ToEmail, lower, StringComparison.OrdinalIgnoreCase)) return Forbid();

            if (!msg.IsRead)
            {
                msg.IsRead = true;
                msg.ReadAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = "Marked as read." });
        }

        // POST /api/mail/reply
        [HttpPost("reply")]
        public async Task<IActionResult> Reply([FromBody] MailReplyDto request)
        {
            var email = await GetCurrentUserEmailAsync();
            if (email == null) return Unauthorized();
            if (request.MessageId == Guid.Empty || string.IsNullOrWhiteSpace(request.Body))
                return BadRequest(new { message = "Message and reply body are required." });

            var source = await _context.MockMailMessages.FirstOrDefaultAsync(m => m.Id == request.MessageId);
            if (source == null) return NotFound(new { message = "Original mail not found." });

            var me = email.ToLowerInvariant();
            if (!string.Equals(source.ToEmail, me, StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(source.FromEmail, me, StringComparison.OrdinalIgnoreCase))
                return Forbid();

            var to = await ResolveReplyRecipientAsync(source, me);
            if (string.Equals(to, me, StringComparison.OrdinalIgnoreCase))
                return BadRequest(new { message = "Cannot send reply to your own inbox. Pick a received message from the other person." });
            var subject = source.Subject.StartsWith("Re:", StringComparison.OrdinalIgnoreCase)
                ? source.Subject
                : $"Re: {source.Subject}";

            await _context.MockMailMessages.AddAsync(new MockMailMessage
            {
                FromEmail = me,
                ToEmail = to.ToLowerInvariant(),
                Subject = subject,
                Body = request.Body.Trim(),
                ParentMessageId = source.Id
            });
            await _context.SaveChangesAsync();

            // Keep real email delivery path untouched as requested.
            try
            {
                await _emailService.SendEmailAsync(to, subject, request.Body.Trim(), saveToMockInbox: false);
            }
            catch
            {
                // SMTP can fail in local/dev setups; mock inbox reply remains persisted.
                return Ok(new { message = "Reply saved in mock inbox. Real email delivery failed in current environment." });
            }

            return Ok(new { message = "Reply sent." });
        }

        private async Task<string> ResolveReplyRecipientAsync(MockMailMessage source, string me)
        {
            var from = (source.FromEmail ?? string.Empty).Trim().ToLowerInvariant();
            var to = (source.ToEmail ?? string.Empty).Trim().ToLowerInvariant();
            var self = me.Trim().ToLowerInvariant();

            if (from == self && to != self) return to;
            if (to == self && from != self)
            {
                if (IsSystemMailbox(from))
                {
                    var admin = await _context.Users
                        .AsNoTracking()
                        .Where(u => u.Role == UserRole.ADMIN)
                        .OrderBy(u => u.CreatedAt)
                        .Select(u => u.Email)
                        .FirstOrDefaultAsync();
                    if (!string.IsNullOrWhiteSpace(admin))
                        return admin.Trim().ToLowerInvariant();
                }
                return from;
            }

            // Fallback: when message data is odd, prefer "other" side if possible.
            if (from != self && !string.IsNullOrWhiteSpace(from)) return from;
            return to;
        }

        private static bool IsSystemMailbox(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return true;
            return email.Contains("no-reply", StringComparison.OrdinalIgnoreCase)
                || email.Contains("gobus.local", StringComparison.OrdinalIgnoreCase);
        }

        // DELETE /api/mail/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var email = await GetCurrentUserEmailAsync();
            if (email == null) return Unauthorized();
            var lower = email.ToLowerInvariant();

            var msg = await _context.MockMailMessages.FirstOrDefaultAsync(m => m.Id == id);
            if (msg == null) return NotFound(new { message = "Mail not found." });

            var isOwner =
                string.Equals(msg.ToEmail, lower, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(msg.FromEmail, lower, StringComparison.OrdinalIgnoreCase);
            if (!isOwner) return Forbid();

            _context.MockMailMessages.Remove(msg);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Mail deleted." });
        }

        private async Task<string?> GetCurrentUserEmailAsync()
        {
            var claim = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue("email");
            if (!string.IsNullOrWhiteSpace(claim)) return claim.Trim();

            var sub = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (!Guid.TryParse(sub, out var uid)) return null;

            var email = await _context.Users
                .Where(u => u.Id == uid)
                .Select(u => u.Email)
                .FirstOrDefaultAsync();
            return email?.Trim();
        }
    }

    public class MailReplyDto
    {
        public Guid MessageId { get; set; }
        public string Body { get; set; } = string.Empty;
    }
}
