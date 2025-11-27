using MediatR;
using ProjectManagementService.Application.DTOs.Meeting;

namespace ProjectManagementService.Application.Features.Meetings.Commands.CreateMeeting;

/// <summary>
/// Command để tạo Meeting mới
/// </summary>
public class CreateMeetingCommand : IRequest<MeetingDto>
{
    public string Title { get; set; } = null!;
    
    public string? Description { get; set; }
    
    public DateTime StartTime { get; set; }
    
    public DateTime EndTime { get; set; }
    
    public string? MeetingLink { get; set; }
    
    public List<string> Attendees { get; set; } = new();
}
