namespace ITHelpDesk.Application.Interfaces.Identity
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string htmlBody);
    }
}
