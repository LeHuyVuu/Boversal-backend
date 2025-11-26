namespace UtilityService.Models;

public class EmailRequestDto
{
    public string ToEmail { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string Content { get; set; } = null!;
}