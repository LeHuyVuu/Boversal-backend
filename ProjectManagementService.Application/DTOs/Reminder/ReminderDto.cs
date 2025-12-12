namespace ProjectManagementService.Application.DTOs.Reminder;

/// <summary>
/// DTO trả về thông tin nhắc hẹn
/// </summary>
public class ReminderDto
{
    public Guid Id { get; set; }
    public long UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Note { get; set; }
    public DateTime ReminderTime { get; set; }
    public bool IsEmailSent { get; set; }
    public DateTime? EmailSentAt { get; set; }
    public bool IsExpired { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
