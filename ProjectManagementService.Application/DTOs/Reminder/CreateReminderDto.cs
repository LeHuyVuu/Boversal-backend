namespace ProjectManagementService.Application.DTOs.Reminder;

/// <summary>
/// DTO để tạo nhắc hẹn mới
/// </summary>
public class CreateReminderDto
{
    public string Title { get; set; } = string.Empty;
    public string? Note { get; set; }
    public DateTime ReminderTime { get; set; }
}
