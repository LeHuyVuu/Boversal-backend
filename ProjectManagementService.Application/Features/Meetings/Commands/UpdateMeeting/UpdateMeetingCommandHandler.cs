using System.Text.Json;
using MediatR;
using ProjectManagementService.Application.Interfaces;

namespace ProjectManagementService.Application.Features.Meetings.Commands.UpdateMeeting;

/// <summary>
/// Handler xử lý UpdateMeetingCommand
/// </summary>
public class UpdateMeetingCommandHandler : IRequestHandler<UpdateMeetingCommand, bool>
{
    private readonly IMeetingRepository _meetingRepository;
    private readonly ICurrentUserService _currentUserService;

    public UpdateMeetingCommandHandler(
        IMeetingRepository meetingRepository,
        ICurrentUserService currentUserService)
    {
        _meetingRepository = meetingRepository;
        _currentUserService = currentUserService;
    }

    public async Task<bool> Handle(UpdateMeetingCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        // Lấy Meeting từ DB
        var meeting = await _meetingRepository.GetByIdAsync(request.MeetingId);
        
        if (meeting == null)
        {
            throw new KeyNotFoundException($"Meeting với ID {request.MeetingId} không tồn tại");
        }

        // Kiểm tra quyền sở hữu
        if (meeting.UserId != userId)
        {
            throw new UnauthorizedAccessException("Bạn không có quyền chỉnh sửa meeting này");
        }

        // Update thông tin
        meeting.Title = request.Title;
        meeting.Description = request.Description;
        meeting.StartTime = request.StartTime;
        meeting.EndTime = request.EndTime;
        meeting.MeetingLink = request.MeetingLink;
        meeting.Attendees = JsonSerializer.Serialize(request.Attendees);

        return await _meetingRepository.UpdateAsync(meeting);
    }
}
