using MediatR;

namespace ProjectManagementService.Application.Features.Meetings.Commands.UpdateMeeting;

/// <summary>
/// Command để update Meeting
/// </summary>
public class UpdateMeetingCommand : IRequest<bool>
{
    public long MeetingId { get; set; }
    
    public string Title { get; set; } = null!;
    
    public string? Description { get; set; }
    
    public DateTime StartTime { get; set; }
    
    public DateTime EndTime { get; set; }
    
    public string? MeetingLink { get; set; }
    
    public List<string> Attendees { get; set; } = new();
}
