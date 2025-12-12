namespace ProjectManagementService.Application.DTOs.Reminder;

/// <summary>
/// DTO để cập nhật nhắc hẹn
/// </summary>
public class UpdateReminderDto
{
    public string? Title { get; set; }
    public string? Note { get; set; }
    public DateTime? ReminderTime { get; set; }
    public bool? IsCompleted { get; set; }
}
