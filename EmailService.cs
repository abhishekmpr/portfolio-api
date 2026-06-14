using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace PortfolioApi
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendLeadEmail(
            string name,
            string email,
            string question)
        {
            var senderEmail =
                _configuration["EmailSettings:Email"];

            var senderPassword =
                _configuration["EmailSettings:Password"];

            var host =
                _configuration["EmailSettings:Host"];

            var port =
                int.Parse(_configuration["EmailSettings:Port"]);

            var message = new MimeMessage();

            message.From.Add(
                MailboxAddress.Parse(senderEmail));

            message.To.Add(
                MailboxAddress.Parse(senderEmail));

            message.Subject = "New Portfolio Lead";

            message.Body = new TextPart("plain")
            {
                Text = $@"
New Portfolio Lead

Name : {name}

Email : {email}

Question :

{question}
"
            };

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(
                host,
                port,
                SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(
                senderEmail,
                senderPassword);

            await smtp.SendAsync(message);

            await smtp.DisconnectAsync(true);
        }
    }
}