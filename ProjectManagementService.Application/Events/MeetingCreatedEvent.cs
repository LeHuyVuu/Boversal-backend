namespace ProjectManagementService.Application.Events;

/// <summary>
/// Event được publish khi Meeting được tạo
/// </summary>
public class MeetingCreatedEvent
{
    public long MeetingId { get; set; }
    
    public string Title { get; set; } = null!;
    
    public string? Description { get; set; }
    
    public DateTime StartTime { get; set; }
    
    public DateTime EndTime { get; set; }
    
    public string? MeetingLink { get; set; }
    
    public string OrganizerEmail { get; set; } = null!;
    
    public string OrganizerName { get; set; } = null!;
    
    public List<string> Attendees { get; set; } = new();
}
