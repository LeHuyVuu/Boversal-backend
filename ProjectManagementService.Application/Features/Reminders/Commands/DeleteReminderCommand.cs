using MediatR;
using ProjectManagementService.Application.Interfaces;

namespace ProjectManagementService.Application.Features.Reminders.Commands;

/// <summary>
/// Command để xóa nhắc hẹn
/// </summary>
public class DeleteReminderCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class DeleteReminderCommandHandler : IRequestHandler<DeleteReminderCommand, bool>
{
    private readonly IReminderRepository _reminderRepository;
    private readonly ICurrentUserService _currentUserService;

    public DeleteReminderCommandHandler(
        IReminderRepository reminderRepository,
        ICurrentUserService currentUserService)
    {
        _reminderRepository = reminderRepository;
        _currentUserService = currentUserService;
    }

    public async System.Threading.Tasks.Task<bool> Handle(DeleteReminderCommand request, CancellationToken cancellationToken)
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

        // Chỉ cho phép xóa reminder của chính mình
        if (reminder.UserId != userId.Value)
        {
            throw new UnauthorizedAccessException("Forbidden");
        }

        await _reminderRepository.DeleteAsync(request.Id, cancellationToken);

        return true;
    }
}
