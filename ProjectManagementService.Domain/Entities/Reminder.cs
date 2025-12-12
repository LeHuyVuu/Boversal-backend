namespace ProjectManagementService.Domain.Entities;

/// <summary>
/// Nhắc hẹn cá nhân - Personal Calendar/Reminder
/// </summary>
public class Reminder
{
    public Guid Id { get; set; }
    
    /// <summary>
    /// User ID - người tạo nhắc hẹn
    /// </summary>
    public long UserId { get; set; }
    
    /// <summary>
    /// Tiêu đề nhắc hẹn
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Ghi chú chi tiết
    /// </summary>
    public string? Note { get; set; }
    
    /// <summary>
    /// Thời gian nhắc hẹn
    /// </summary>
    public DateTime ReminderTime { get; set; }
    
    /// <summary>
    /// Email đã được gửi nhắc trước 15 phút chưa
    /// </summary>
    public bool IsEmailSent { get; set; } = false;
    
    /// <summary>
    /// Thời gian gửi email nhắc
    /// </summary>
    public DateTime? EmailSentAt { get; set; }
    
    /// <summary>
    /// Đã hết hạn chưa (sau thời gian hẹn)
    /// </summary>
    public bool IsExpired { get; set; } = false;
    
    /// <summary>
    /// Đã hoàn thành/đã xử lý chưa
    /// </summary>
    public bool IsCompleted { get; set; } = false;
    
    /// <summary>
    /// Ngày tạo
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Ngày cập nhật
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
    
    /// <summary>
    /// Navigation property - User
    /// </summary>
    public virtual User User { get; set; } = null!;
}
