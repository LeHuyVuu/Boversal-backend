namespace ProjectManagementService.Application.DTOs.Meeting;

/// <summary>
/// DTO trả về thông tin Meeting
/// </summary>
public class MeetingDto
{
    public long Id { get; set; }
    
    public string Title { get; set; } = null!;
    
    public string? Description { get; set; }
    
    public DateTime StartTime { get; set; }
    
    public DateTime EndTime { get; set; }
    
    public string? MeetingLink { get; set; }
    
    public long UserId { get; set; }
    
    /// <summary>
    /// Danh sách email người tham gia
    /// </summary>
    public List<string> Attendees { get; set; } = new();
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// DTO để tạo Meeting mới
/// </summary>
public class CreateMeetingDto
{
    public string Title { get; set; } = null!;
    
    public string? Description { get; set; }
    
    public DateTime StartTime { get; set; }
    
    public DateTime EndTime { get; set; }
    
    public string? MeetingLink { get; set; }
    
    /// <summary>
    /// Danh sách email người tham gia
    /// </summary>
    public List<string> Attendees { get; set; } = new();
}

/// <summary>
/// DTO để update Meeting
/// </summary>
public class UpdateMeetingDto
{
    public string Title { get; set; } = null!;
    
    public string? Description { get; set; }
    
    public DateTime StartTime { get; set; }
    
    public DateTime EndTime { get; set; }
    
    public string? MeetingLink { get; set; }
    
    public List<string> Attendees { get; set; } = new();
}
