using UtilityService.Infrastructure.Repositories;
using UtilityService.Models;

namespace UtilityService.Infrastructure.Services;

public class EmailService
{
    private readonly EmailRepository _emailRepo;
    private readonly ILogger<EmailService> _logger;

    public EmailService(EmailRepository emailRepo, ILogger<EmailService> logger)
    {
        _emailRepo = emailRepo;
        _logger = logger;
    }

    public async Task<ApiResponse<string>> SendEmailAsync(EmailRequestDto dto)
    {
      try
      {
        // Build HTML email body
        string htmlBody = @"
<html>
<head>
  <meta charset=""utf-8"" />
  <style>
    body {
      font-family: 'Segoe UI', Roboto, Arial, sans-serif;
      background: #f4f6fb;
      padding: 40px;
      color: #1f2937;
      margin: 0;
    }
    .container {
      background: #ffffff;
      padding: 32px;
      border-radius: 16px;
      max-width: 650px;
      margin: 0 auto;
      box-shadow: 0 4px 16px rgba(0, 0, 0, 0.08);
      border: 1px solid #e5e7eb;
    }
    h2 {
      color: #4338ca;
      border-bottom: 2px solid #6366f1;
      padding-bottom: 10px;
      margin-bottom: 20px;
      font-size: 22px;
      font-weight: 600;
    }
    .content {
      line-height: 1.6;
      color: #374151;
      font-size: 15px;
    }
    .footer {
      margin-top: 35px;
      font-size: 13px;
      color: #9ca3af;
      text-align: center;
      border-top: 1px solid #e5e7eb;
      padding-top: 15px;
    }
  </style>
</head>
<body>
  <div class=""container"">
    <h2>" + dto.Subject + @"</h2>
    <div class=""content"">" + dto.Content + @"</div>
    <div class=""footer"">© " + DateTime.UtcNow.Year + @" Boversal System. All rights reserved.</div>
  </div>
</body>
</html>";

        var ok = await _emailRepo.SendEmailAsync(dto.ToEmail, dto.Subject, htmlBody);
        return ok
          ? ApiResponse<string>.Success("Email sent successfully")
          : ApiResponse<string>.Fail(500, "Failed to send email");
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "EmailService.SendEmailAsync error");
        return ApiResponse<string>.Fail(500, "Error sending email", ex.Message);
      }
    }
}
