using System.Net;
using System.Net.Mail;
using RailwayReservation.Interfaces;

namespace RailwayReservation.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            // Pulling credentials from appsettings.json for security
            var smtpHost = _config["EmailSettings:Host"];
            var smtpPort = int.Parse(_config["EmailSettings:Port"]!);
            var smtpEmail = _config["EmailSettings:Username"];
            var smtpPass = _config["EmailSettings:Password"];

            using (var client = new SmtpClient(smtpHost, smtpPort))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(smtpEmail, smtpPass);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpEmail!),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(toEmail);

                // Use the Async version to prevent blocking the thread
                await client.SendMailAsync(mailMessage);
            }
        }
    }
}