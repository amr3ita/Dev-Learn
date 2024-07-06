using Code_Road.Helpers;
using Code_Road.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Code_Road.Services.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly UrlHelperFactoryService _urlHelperFactoryService;

        public EmailService(IOptions<EmailSettings> emailSettings, UrlHelperFactoryService urlHelperFactoryService)
        {
            _emailSettings = emailSettings.Value;
            _urlHelperFactoryService = urlHelperFactoryService;
        }
        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Code Road Web", _emailSettings.SmtpUser));
            emailMessage.To.Add(MailboxAddress.Parse(toEmail));
            emailMessage.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = message };
            emailMessage.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_emailSettings.SmtpUser, _emailSettings.SmtpPass);
                await client.SendAsync(emailMessage);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error sending email: {ex.Message}");
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }
    }
}