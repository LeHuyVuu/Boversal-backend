using Microsoft.Extensions.Logging;
using ProjectManagementService.Application.Interfaces;

namespace ProjectManagementService.Infrastructure.Services;

/// <summary>
/// Email service implementation using SMTP
/// Replace with SendGrid/AWS SES in production
/// </summary>
public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
    {
        // TODO: Implement actual email sending logic
        // Example: Using MailKit, SendGrid, AWS SES, etc.
        
        _logger.LogInformation(
            "Sending email to {To}, Subject: {Subject}",
            to,
            subject);

        await Task.CompletedTask;
    }

    public async Task SendEmailAsync(List<string> recipients, string subject, string body, bool isHtml = true)
    {
        foreach (var recipient in recipients)
        {
            await SendEmailAsync(recipient, subject, body, isHtml);
        }
    }

    public async Task SendTemplateEmailAsync(string to, string templateId, object templateData)
    {
        // TODO: Implement template-based email sending
        
        _logger.LogInformation(
            "Sending template email to {To}, Template: {TemplateId}",
            to,
            templateId);

        await Task.CompletedTask;
    }
}
