using MailKit.Net.Smtp;
using MimeKit;

public class EmailService
{
    public async Task SendLeadEmail(
        string name,
        string email,
        string question)
    {
        var message = new MimeMessage();

        message.From.Add(
            MailboxAddress.Parse("your-email@gmail.com"));

        message.To.Add(
            MailboxAddress.Parse(
                "aaabhishekmishra123@gmail.com"));

        message.Subject = "New Portfolio Lead";

        message.Body = new TextPart("plain")
        {
            Text = $@"
Name: {name}

Email: {email}

Question:
{question}
"
        };

        using var smtp = new SmtpClient();

        await smtp.ConnectAsync(
            "smtp.gmail.com",
            587,
            MailKit.Security.SecureSocketOptions.StartTls);

        await smtp.AuthenticateAsync(
    "aaabhishekmishra123@gmail.com",
    "fhov xjku lyoj lhkz"
);
        await smtp.SendAsync(message);

        await smtp.DisconnectAsync(true);
    }
}