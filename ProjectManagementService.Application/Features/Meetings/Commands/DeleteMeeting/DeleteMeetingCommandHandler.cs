using MediatR;
using ProjectManagementService.Application.Interfaces;

namespace ProjectManagementService.Application.Features.Meetings.Commands.DeleteMeeting;

/// <summary>
/// Handler xử lý DeleteMeetingCommand
/// </summary>
public class DeleteMeetingCommandHandler : IRequestHandler<DeleteMeetingCommand, bool>
{
    private readonly IMeetingRepository _meetingRepository;
    private readonly ICurrentUserService _currentUserService;

    public DeleteMeetingCommandHandler(
        IMeetingRepository meetingRepository,
        ICurrentUserService currentUserService)
    {
        _meetingRepository = meetingRepository;
        _currentUserService = currentUserService;
    }

    public async Task<bool> Handle(DeleteMeetingCommand request, CancellationToken cancellationToken)
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
            throw new UnauthorizedAccessException("Bạn không có quyền xóa meeting này");
        }

        return await _meetingRepository.DeleteAsync(request.MeetingId);
    }
}
