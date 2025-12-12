using MediatR;
using ProjectManagementService.Application.DTOs.Reminder;
using ProjectManagementService.Application.Interfaces;
using ProjectManagementService.Domain.Entities;

namespace ProjectManagementService.Application.Features.Reminders.Commands;

/// <summary>
/// Command để tạo nhắc hẹn mới
/// </summary>
public class CreateReminderCommand : IRequest<ReminderDto>
{
    public string Title { get; set; } = string.Empty;
    public string? Note { get; set; }
    public DateTime ReminderTime { get; set; }
}

public class CreateReminderCommandHandler : IRequestHandler<CreateReminderCommand, ReminderDto>
{
    private readonly IReminderRepository _reminderRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateReminderCommandHandler(
        IReminderRepository reminderRepository,
        ICurrentUserService currentUserService)
    {
        _reminderRepository = reminderRepository;
        _currentUserService = currentUserService;
    }

    public async System.Threading.Tasks.Task<ReminderDto> Handle(CreateReminderCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue || userId.Value == 0)
        {
            throw new UnauthorizedAccessException("User not authenticated");
        }

        var reminder = new Reminder
        {
            UserId = userId.Value,
            Title = request.Title,
            Note = request.Note,
            ReminderTime = request.ReminderTime,
            IsEmailSent = false,
            IsExpired = false,
            IsCompleted = false
        };

        var created = await _reminderRepository.CreateAsync(reminder, cancellationToken);

        return new ReminderDto
        {
            Id = created.Id,
            UserId = created.UserId,
            Title = created.Title,
            Note = created.Note,
            ReminderTime = created.ReminderTime,
            IsEmailSent = created.IsEmailSent,
            EmailSentAt = created.EmailSentAt,
            IsExpired = created.IsExpired,
            IsCompleted = created.IsCompleted,
            CreatedAt = created.CreatedAt,
            UpdatedAt = created.UpdatedAt
        };
    }
}
