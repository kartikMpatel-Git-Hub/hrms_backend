
using hrms.Utility;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace hrms.Service.impl
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var message = new MailMessage
            {
                From = new MailAddress(_settings.From),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            message.To.Add(to);

            var Client = new SmtpClient(_settings.Host, _settings.Port)
            {
                Credentials = new NetworkCredential(
                    _settings.Username,
                    _settings.Password),
                EnableSsl = _settings.EnableSsl
            };

            await Client.SendMailAsync(message);
            Console.WriteLine($"Email Delivered To {to}");
        }
    }
}
