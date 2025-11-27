using Microsoft.AspNetCore.Mvc;
using UtilityService.Models;
using EmailServiceAlias = UtilityService.Infrastructure.Services.EmailService;

namespace UtilityService.Infrastructure.Controller;

[ApiController]
[Route("api/[controller]")]
public class EmailController : ControllerBase
{
    private readonly EmailServiceAlias _emailService;
    
    public EmailController(EmailServiceAlias emailService) => _emailService = emailService;

    /// <summary>
    /// Send email
    /// </summary>
    /// <param name="dto">Email request with ToEmail, Subject, Content</param>
    /// <returns>ApiResponse with status</returns>
    [HttpPost("send")]
    public async Task<IActionResult> SendEmail([FromBody] EmailRequestDto dto)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.ToEmail))
            return BadRequest(ApiResponse<string>.Fail(400, "Invalid request: ToEmail is required"));

        if (string.IsNullOrWhiteSpace(dto.Subject))
            return BadRequest(ApiResponse<string>.Fail(400, "Invalid request: Subject is required"));

        var result = await _emailService.SendEmailAsync(dto);
        return StatusCode(result.Status, result);
    }
}