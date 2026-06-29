namespace ITHelpDesk.Application.Interfaces.Identity
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string toEmail, string subject, string htmlBody, List<string>? ccEmails = null);
    }
}
