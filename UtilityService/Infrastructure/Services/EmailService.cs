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
        // Lưu ý: nếu dto.Content chứa HTML do user nhập, hãy cân nhắc sanitize trước khi gửi.
        string htmlBody = $@"
<html>
<head>
  <meta charset=""utf-8"" />
  <style>
    body {{
      font-family: 'Segoe UI', Roboto, Arial, sans-serif;
      background: #f4f6fb;
      padding: 40px;
      color: #1f2937;
      margin: 0;
    }}
    .container {{
      background: #ffffff;
      padding: 32px;
      border-radius: 16px;
      max-width: 650px;
      margin: 0 auto;
      box-shadow: 0 4px 16px rgba(0, 0, 0, 0.08);
      border: 1px solid #e5e7eb;
    }}
    h2 {{
      color: #4338ca;
      border-bottom: 2px solid #6366f1;
      padding-bottom: 10px;
      margin-bottom: 20px;
      font-size: 22px;
      font-weight: 600;
    }}
    .content {{
      line-height: 1.6;
      color: #374151;
      font-size: 15px;
    }}
    .button {{
      display: inline-block;
      background: linear-gradient(135deg, #6366f1, #4f46e5);
      color: #ffffff !important;
      padding: 12px 24px;
      border-radius: 8px;
      text-decoration: none;
      font-weight: 500;
      margin-top: 25px;
      transition: background 0.3s ease;
    }}
    .button:hover {{
      background: linear-gradient(135deg, #4f46e5, #4338ca);
    }}
    .footer {{
      margin-top: 35px;
      font-size: 13px;
      color: #9ca3af;
      text-align: center;
      border-top: 1px solid #e5e7eb;
      padding-top: 15px;
    }}
    a {{
      color: #4f46e5;
      text-decoration: none;
    }}
    a:hover {{
      text-decoration: underline;
    }}
  </style>
</head>
<body>
  <div class=""container"">
    <h2>{dto.Subject}</h2>
    <div class=""content"">{dto.Content}</div>
    <p>
      <a class=""button"" href=""https://ev-management-frontend.vercel.app/feedbacks"" target=""_blank"" rel=""noreferrer"">
        Gửi khiếu nại về EVM
      </a>
    </p>
    <div class=""footer"">© {DateTime.UtcNow.Year} EV Management System. All rights reserved.</div>
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