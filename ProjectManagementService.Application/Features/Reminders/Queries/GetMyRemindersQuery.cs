using MediatR;
using ProjectManagementService.Application.DTOs.Reminder;
using ProjectManagementService.Application.Interfaces;

namespace ProjectManagementService.Application.Features.Reminders.Queries;

/// <summary>
/// Query để lấy danh sách nhắc hẹn của user
/// </summary>
public class GetMyRemindersQuery : IRequest<List<ReminderDto>>
{
    public bool? IncludeExpired { get; set; } = false;
    public bool? IncludeCompleted { get; set; } = false;
}

public class GetMyRemindersQueryHandler : IRequestHandler<GetMyRemindersQuery, List<ReminderDto>>
{
    private readonly IReminderRepository _reminderRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetMyRemindersQueryHandler(
        IReminderRepository reminderRepository,
        ICurrentUserService currentUserService)
    {
        _reminderRepository = reminderRepository;
        _currentUserService = currentUserService;
    }

    public async System.Threading.Tasks.Task<List<ReminderDto>> Handle(GetMyRemindersQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue || userId.Value == 0)
        {
            throw new UnauthorizedAccessException("User not authenticated");
        }

        var reminders = await _reminderRepository.GetByUserIdAsync(userId.Value, cancellationToken);

        // Filter theo yêu cầu
        var filtered = reminders.AsEnumerable();
        
        if (!request.IncludeExpired.GetValueOrDefault())
        {
            filtered = filtered.Where(r => !r.IsExpired);
        }
        
        if (!request.IncludeCompleted.GetValueOrDefault())
        {
            filtered = filtered.Where(r => !r.IsCompleted);
        }

        return filtered.Select(r => new ReminderDto
        {
            Id = r.Id,
            UserId = r.UserId,
            Title = r.Title,
            Note = r.Note,
            ReminderTime = r.ReminderTime,
            IsEmailSent = r.IsEmailSent,
            EmailSentAt = r.EmailSentAt,
            IsExpired = r.IsExpired,
            IsCompleted = r.IsCompleted,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt
        }).ToList();
    }
}
