using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace OnlinePractice.API.Services
{
    public class SendGridServices : ISendGridServices
    {
        private readonly IConfiguration _configuration;
        public SendGridServices(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //private static void Main()
        //{
        //    Execute().Wait();
        //}

        public async Task Execute()
        {
            var apiKey = _configuration.GetValue<string>("SendGrid:APIKey");
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(_configuration.GetValue<string>("SendGrid:FromEmail"), "Example User");
            var subject = "Sending with SendGrid is Fun";
            var to = new EmailAddress("test@example.com", "Example User");
            var plainTextContent = "and easy to do anywhere, even with C#";
            var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
    }
}
