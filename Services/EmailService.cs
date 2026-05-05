using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using PolyTrack.API.Models;
using System.Threading.Tasks;

namespace PolyTrack.API.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_emailSettings.SenderEmail);
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = body };
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            try
            {
                // Remove spaces from password if any (App Passwords have spaces sometimes)
                var cleanPassword = _emailSettings.Password.Replace(" ", "");

                await smtp.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.Port, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_emailSettings.Username, cleanPassword);
                await smtp.SendAsync(email);
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine("-----------------------------------------");
                System.Console.WriteLine($"!!! EMAIL FAILED !!!");
                System.Console.WriteLine($"Error: {ex.Message}");
                if (ex.InnerException != null) System.Console.WriteLine($"Inner: {ex.InnerException.Message}");
                System.Console.WriteLine("-----------------------------------------");
            }
            finally
            {
                await smtp.DisconnectAsync(true);
            }
        }
    }
}
