namespace OnlinePractice.API.Models.AuthDB
{
    public interface IEmailService
    {
        Task<bool> SendEmail(EmailData emailData);
    }
}
