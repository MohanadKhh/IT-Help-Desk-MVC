using ITHelpDesk.Application.Interfaces.Identity;
using ITHelpDesk.Application.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace ITHelpDesk.Infrastructure.Services.Identity;

public sealed class EmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailSettings> settings, ILogger<EmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<bool> SendEmailAsync(string toEmail, string subject, string htmlBody, List<string>? ccEmails = null)
    {
        try
        {
            var client = new SendGridClient(_settings.ApiKey);

            var from = new EmailAddress(_settings.SenderEmail, _settings.SenderName);
            var to = new EmailAddress(toEmail);

            var message = MailHelper.CreateSingleEmail(
                from, to, subject,
                plainTextContent: string.Empty,
                htmlContent: htmlBody);

            // Add CC recipients if provided
            if (ccEmails is { Count: > 0 })
            {
                foreach (var cc in ccEmails)
                    message.AddCc(new EmailAddress(cc));
            }

            var response = await client.SendEmailAsync(message);

            if ((int)response.StatusCode >= 400)
            {
                var body = await response.Body.ReadAsStringAsync();
                _logger.LogError("SendGrid failed: {Status} - {Content}", response.StatusCode, body);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
            return false;
        }
    }
}
