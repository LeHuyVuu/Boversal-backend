namespace ProjectManagementService.Application.Interfaces;

/// <summary>
/// Email service interface - implement in Infrastructure layer
/// Example: SendGrid, AWS SES, SMTP
/// </summary>
public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);
    Task SendEmailAsync(List<string> recipients, string subject, string body, bool isHtml = true);
    Task SendTemplateEmailAsync(string to, string templateId, object templateData);
}
