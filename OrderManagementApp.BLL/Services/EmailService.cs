using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using OrderManagementApp.BLL.Interfaces;
using OrderManagementApp.Common.Settings;

namespace OrderManagementApp.BLL.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtpSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<SmtpSettings> smtpSettings, ILogger<EmailService> logger)
        {
            _smtpSettings = smtpSettings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody, CancellationToken ct = default)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_smtpSettings.SenderName, _smtpSettings.SenderEmail));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;
            message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

            using var client = new SmtpClient();

            try
            {
                var socketOptions = _smtpSettings.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto;

                await client.ConnectAsync(_smtpSettings.Host, _smtpSettings.Port, socketOptions, ct);
                await client.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password, ct);
                await client.SendAsync(message, ct);

                _logger.LogInformation("Email sent successfully to {ToEmail}.", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending email to {ToEmail}.", toEmail);
                throw;
            }
            finally
            {
                if (client.IsConnected)
                    await client.DisconnectAsync(true, ct);
            }
        }
    }
}
