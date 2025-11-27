using System.Text.Json;
using MediatR;
using ProjectManagementService.Application.DTOs.Meeting;
using ProjectManagementService.Application.Interfaces;

namespace ProjectManagementService.Application.Features.Meetings.Queries.GetMeetings;

/// <summary>
/// Handler xử lý GetMeetingsQuery
/// </summary>
public class GetMeetingsQueryHandler : IRequestHandler<GetMeetingsQuery, List<MeetingDto>>
{
    private readonly IMeetingRepository _meetingRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetMeetingsQueryHandler(
        IMeetingRepository meetingRepository,
        ICurrentUserService currentUserService)
    {
        _meetingRepository = meetingRepository;
        _currentUserService = currentUserService;
    }

    public async Task<List<MeetingDto>> Handle(GetMeetingsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId ?? throw new UnauthorizedAccessException("User not authenticated");

        var meetings = await _meetingRepository.GetUserMeetingsAsync(userId);

        return meetings.Select(m => new MeetingDto
        {
            Id = m.Id,
            Title = m.Title,
            Description = m.Description,
            StartTime = m.StartTime,
            EndTime = m.EndTime,
            MeetingLink = m.MeetingLink,
            UserId = m.UserId,
            Attendees = JsonSerializer.Deserialize<List<string>>(m.Attendees) ?? new List<string>(),
            CreatedAt = m.CreatedAt,
            UpdatedAt = m.UpdatedAt
        }).ToList();
    }
}
