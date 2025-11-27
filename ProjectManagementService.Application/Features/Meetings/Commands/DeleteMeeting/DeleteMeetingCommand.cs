using MediatR;

namespace ProjectManagementService.Application.Features.Meetings.Commands.DeleteMeeting;

/// <summary>
/// Command để xóa Meeting
/// </summary>
public class DeleteMeetingCommand : IRequest<bool>
{
    public long MeetingId { get; set; }
}
