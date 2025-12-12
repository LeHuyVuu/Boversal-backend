using MediatR;
using ProjectManagementService.Application.DTOs.Reminder;
using ProjectManagementService.Application.Interfaces;

namespace ProjectManagementService.Application.Features.Reminders.Commands;

/// <summary>
/// Command để cập nhật nhắc hẹn
/// </summary>
public class UpdateReminderCommand : IRequest<ReminderDto>
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Note { get; set; }
    public DateTime? ReminderTime { get; set; }
    public bool? IsCompleted { get; set; }
}

public class UpdateReminderCommandHandler : IRequestHandler<UpdateReminderCommand, ReminderDto>
{
    private readonly IReminderRepository _reminderRepository;
    private readonly ICurrentUserService _currentUserService;

    public UpdateReminderCommandHandler(
        IReminderRepository reminderRepository,
        ICurrentUserService currentUserService)
    {
        _reminderRepository = reminderRepository;
        _currentUserService = currentUserService;
    }

    public async System.Threading.Tasks.Task<ReminderDto> Handle(UpdateReminderCommand request, CancellationToken cancellationToken)
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

        // Chỉ cho phép update reminder của chính mình
        if (reminder.UserId != userId.Value)
        {
            throw new UnauthorizedAccessException("Forbidden");
        }

        // Không cho phép update nếu đã hết hạn
        if (reminder.IsExpired)
        {
            throw new InvalidOperationException("Cannot update expired reminder");
        }

        // Không cho phép update thời gian nếu đã qua
        if (request.ReminderTime.HasValue && request.ReminderTime.Value < DateTime.UtcNow)
        {
            throw new InvalidOperationException("Cannot set reminder time in the past");
        }

        // Update các trường
        if (!string.IsNullOrEmpty(request.Title))
            reminder.Title = request.Title;
        
        if (request.Note != null)
            reminder.Note = request.Note;
        
        if (request.ReminderTime.HasValue)
        {
            reminder.ReminderTime = request.ReminderTime.Value;
            // Reset email sent flag nếu đổi thời gian
            reminder.IsEmailSent = false;
            reminder.EmailSentAt = null;
        }
        
        if (request.IsCompleted.HasValue)
            reminder.IsCompleted = request.IsCompleted.Value;

        var updated = await _reminderRepository.UpdateAsync(reminder, cancellationToken);

        return new ReminderDto
        {
            Id = updated.Id,
            UserId = updated.UserId,
            Title = updated.Title,
            Note = updated.Note,
            ReminderTime = updated.ReminderTime,
            IsEmailSent = updated.IsEmailSent,
            EmailSentAt = updated.EmailSentAt,
            IsExpired = updated.IsExpired,
            IsCompleted = updated.IsCompleted,
            CreatedAt = updated.CreatedAt,
            UpdatedAt = updated.UpdatedAt
        };
    }
}
