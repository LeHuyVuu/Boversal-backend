using MailKit.Net.Smtp;
using MimeKit;
using UtilityService.Messaging;

namespace UtilityService.Infrastructure;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendMeetingInvitationAsync(MeetingCreatedEvent meetingEvent, string recipientEmail)
    {
        try
        {
            _logger.LogInformation("Starting to send meeting invitation to {Email} for meeting {MeetingId}", 
                recipientEmail, meetingEvent.MeetingId);

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(
                _configuration["Email:FromName"] ?? "Boversal Meeting",
                _configuration["Email:FromAddress"] ?? "noreply@boversal.com"
            ));
            message.To.Add(MailboxAddress.Parse(recipientEmail));
            message.Subject = $"Meeting Invitation: {meetingEvent.Title}";

            _logger.LogInformation("Email message created with subject: {Subject}", message.Subject);

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = GenerateMeetingEmailTemplate(meetingEvent)
            };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            
            var smtpHost = _configuration["Email:SmtpHost"];
            var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
            var smtpUser = _configuration["Email:SmtpUser"];
            var smtpPass = _configuration["Email:SmtpPassword"];

            _logger.LogInformation("Connecting to SMTP server {Host}:{Port}", smtpHost, smtpPort);
            await client.ConnectAsync(smtpHost, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
            
            _logger.LogInformation("Authenticating with user {User}", smtpUser);
            await client.AuthenticateAsync(smtpUser, smtpPass);
            
            _logger.LogInformation("Sending email...");
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("✅ Meeting invitation SUCCESSFULLY sent to {Email} for meeting {MeetingId}", 
                recipientEmail, meetingEvent.MeetingId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ FAILED to send meeting invitation to {Email}. Error: {ErrorMessage}", 
                recipientEmail, ex.Message);
            throw;
        }
    }

    private string GenerateMeetingEmailTemplate(MeetingCreatedEvent meeting)
    {
        var startTimeFormatted = meeting.StartTime.ToString("dddd, MMMM dd, yyyy 'at' h:mm tt");
        var endTimeFormatted = meeting.EndTime.ToString("h:mm tt");

        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
        .meeting-details {{ background: white; padding: 20px; border-radius: 8px; margin: 20px 0; border-left: 4px solid #667eea; }}
        .detail-row {{ margin: 10px 0; }}
        .label {{ font-weight: bold; color: #667eea; }}
        .meeting-link {{ display: inline-block; background: #667eea; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
        .footer {{ text-align: center; color: #666; font-size: 12px; margin-top: 30px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>📅 Meeting Invitation</h1>
        </div>
        <div class='content'>
            <p>Hello,</p>
            <p>You have been invited to a meeting by <strong>{meeting.OrganizerName}</strong> ({meeting.OrganizerEmail}).</p>
            
            <div class='meeting-details'>
                <h2 style='color: #667eea; margin-top: 0;'>{meeting.Title}</h2>
                
                {(string.IsNullOrEmpty(meeting.Description) ? "" : $"<div class='detail-row'><div class='label'>Description:</div><p>{meeting.Description}</p></div>")}
                
                <div class='detail-row'>
                    <span class='label'>📅 Date & Time:</span><br>
                    {startTimeFormatted} - {endTimeFormatted}
                </div>
                
                <div class='detail-row'>
                    <span class='label'>👤 Organizer:</span><br>
                    {meeting.OrganizerName} ({meeting.OrganizerEmail})
                </div>
                
                {(string.IsNullOrEmpty(meeting.MeetingLink) ? "" : $@"
                <div class='detail-row' style='text-align: center;'>
                    <a href='{meeting.MeetingLink}' class='meeting-link'>🔗 Join Meeting</a>
                </div>")}
            </div>
            
            <p>Please make sure to mark your calendar for this meeting.</p>
            
            <div class='footer'>
                <p>This is an automated email from Boversal Meeting System.</p>
                <p>© 2025 Boversal. All rights reserved.</p>
            </div>
        </div>
    </div>
</body>
</html>";
    }
}
