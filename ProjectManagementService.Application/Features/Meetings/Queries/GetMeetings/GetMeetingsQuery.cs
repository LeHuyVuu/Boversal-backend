using MediatR;
using ProjectManagementService.Application.DTOs.Meeting;

namespace ProjectManagementService.Application.Features.Meetings.Queries.GetMeetings;

/// <summary>
/// Query để lấy danh sách Meeting của user hiện tại
/// </summary>
public class GetMeetingsQuery : IRequest<List<MeetingDto>>
{
}
