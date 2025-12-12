using MediatR;
using ProjectManagementService.Application.DTOs.Reminder;
using ProjectManagementService.Application.Interfaces;

namespace ProjectManagementService.Application.Features.Reminders.Queries;

/// <summary>
/// Query để lấy chi tiết 1 nhắc hẹn
/// </summary>
public class GetReminderByIdQuery : IRequest<ReminderDto>
{
    public Guid Id { get; set; }
}

public class GetReminderByIdQueryHandler : IRequestHandler<GetReminderByIdQuery, ReminderDto>
{
    private readonly IReminderRepository _reminderRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetReminderByIdQueryHandler(
        IReminderRepository reminderRepository,
        ICurrentUserService currentUserService)
    {
        _reminderRepository = reminderRepository;
        _currentUserService = currentUserService;
    }

    public async System.Threading.Tasks.Task<ReminderDto> Handle(GetReminderByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue || userId.Value == 0)
        {
            throw new UnauthorizedAccessException("User not authenticated");
        }

        var reminder = await _reminderRepository.GetByIdAsync(request.Id, cancellationToken);
        if (reminder == null)
        {
            throw new KeyNotFoundException("Reminder not found");
        }

        // Chỉ cho phép xem reminder của chính mình
        if (reminder.UserId != userId.Value)
        {
            throw new UnauthorizedAccessException("Forbidden");
        }

        return new ReminderDto
        {
            Id = reminder.Id,
            UserId = reminder.UserId,
            Title = reminder.Title,
            Note = reminder.Note,
            ReminderTime = reminder.ReminderTime,
            IsEmailSent = reminder.IsEmailSent,
            EmailSentAt = reminder.EmailSentAt,
            IsExpired = reminder.IsExpired,
            IsCompleted = reminder.IsCompleted,
            CreatedAt = reminder.CreatedAt,
            UpdatedAt = reminder.UpdatedAt
        };
    }
}
