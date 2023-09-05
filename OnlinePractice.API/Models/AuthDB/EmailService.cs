using Microsoft.Extensions.Options;
using OnlinePractice.API.Models.DBModel;
using MailKit.Security;
using MailKit.Net.Smtp;
using MimeKit;

namespace OnlinePractice.API.Models.AuthDB
{

    public class EmailService : IEmailService
    {
     private readonly   EmailSettings _emailSettings;
        public EmailService(IOptions<EmailSettings> options)
        {
            _emailSettings = options.Value;
        }
        public async Task<bool> SendEmail(EmailData emailData)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_emailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(emailData.ToEmail));
            email.Subject = emailData.Subject;
            var builder = new BodyBuilder();
            if (emailData.Attachments != null)
            {
                byte[] fileBytes;
                foreach (var file in emailData.Attachments)
                {
                    if (file.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            fileBytes = ms.ToArray();
                        }
                        builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                    }
                }
            }
            builder.HtmlBody = emailData.Body;
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            smtp.Connect(_emailSettings.Host, _emailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_emailSettings.Mail, _emailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
            return true;
        }
    }
}
