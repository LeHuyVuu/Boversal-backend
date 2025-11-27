using MediatR;
using ProjectManagementService.Application.DTOs.Meeting;

namespace ProjectManagementService.Application.Features.Meetings.Queries.GetMeetingById;

/// <summary>
/// Query để lấy Meeting theo ID
/// </summary>
public class GetMeetingByIdQuery : IRequest<MeetingDto?>
{
    public long MeetingId { get; set; }
}
