using System.Text.Json;
using MediatR;
using ProjectManagementService.Application.DTOs.Meeting;
using ProjectManagementService.Application.Interfaces;

namespace ProjectManagementService.Application.Features.Meetings.Queries.GetMeetingById;

/// <summary>
/// Handler xử lý GetMeetingByIdQuery
/// </summary>
public class GetMeetingByIdQueryHandler : IRequestHandler<GetMeetingByIdQuery, MeetingDto?>
{
    private readonly IMeetingRepository _meetingRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetMeetingByIdQueryHandler(
        IMeetingRepository meetingRepository,
        ICurrentUserService currentUserService)
    {
        _meetingRepository = meetingRepository;
        _currentUserService = currentUserService;
    }

    public async Task<MeetingDto?> Handle(GetMeetingByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        var meeting = await _meetingRepository.GetByIdAsync(request.MeetingId);

        if (meeting == null || meeting.UserId != userId)
        {
            return null;
        }

        return new MeetingDto
        {
            Id = meeting.Id,
            Title = meeting.Title,
            Description = meeting.Description,
            StartTime = meeting.StartTime,
            EndTime = meeting.EndTime,
            MeetingLink = meeting.MeetingLink,
            UserId = meeting.UserId,
            Attendees = JsonSerializer.Deserialize<List<string>>(meeting.Attendees) ?? new List<string>(),
            CreatedAt = meeting.CreatedAt,
            UpdatedAt = meeting.UpdatedAt
        };
    }
}
