using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProjectManagementService.Application.Interfaces;

namespace ProjectManagementService.Infrastructure.Services;

/// <summary>
/// Email service implementation using SMTP
/// </summary>
public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly IConfiguration _configuration;

    public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
    {
        try
        {
            var smtpHost = Environment.GetEnvironmentVariable("EMAIL_SMTP_HOST") ?? "smtp.gmail.com";
            var smtpPort = int.Parse(Environment.GetEnvironmentVariable("EMAIL_SMTP_PORT") ?? "587");
            var smtpUser = Environment.GetEnvironmentVariable("EMAIL_SMTP_USER") ?? "";
            var smtpPassword = Environment.GetEnvironmentVariable("EMAIL_SMTP_PASSWORD") ?? "";
            var fromAddress = Environment.GetEnvironmentVariable("EMAIL_FROM_ADDRESS") ?? smtpUser;
            var fromName = Environment.GetEnvironmentVariable("EMAIL_FROM_NAME") ?? "Boversal";

            if (string.IsNullOrEmpty(smtpUser) || string.IsNullOrEmpty(smtpPassword))
            {
                _logger.LogWarning("SMTP credentials not configured. Email not sent to {To}", to);
                return;
            }

            using var smtpClient = new SmtpClient(smtpHost, smtpPort)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(smtpUser, smtpPassword)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromAddress, fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };
            mailMessage.To.Add(to);

            await smtpClient.SendMailAsync(mailMessage);

            _logger.LogInformation("✅ Email sent successfully to {To}, Subject: {Subject}", to, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to send email to {To}", to);
            throw;
        }
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
