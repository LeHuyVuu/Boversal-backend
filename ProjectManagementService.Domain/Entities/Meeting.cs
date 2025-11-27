namespace ProjectManagementService.Domain.Entities;

/// <summary>
/// Entity cho Meeting - Cuộc họp của user
/// </summary>
public class Meeting
{
    public long Id { get; set; }
    
    public string Title { get; set; } = null!;
    
    public string? Description { get; set; }
    
    public DateTime StartTime { get; set; }
    
    public DateTime EndTime { get; set; }
    
    public string? MeetingLink { get; set; }
    
    public long UserId { get; set; }
    
    /// <summary>
    /// JSON array of attendee emails: ["email1@example.com", "email2@example.com"]
    /// </summary>
    public string Attendees { get; set; } = "[]";
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
    
    public DateTime? DeletedAt { get; set; }
    
    // Navigation property
    public User User { get; set; } = null!;
}
