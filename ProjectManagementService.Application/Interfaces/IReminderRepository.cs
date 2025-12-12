using ProjectManagementService.Domain.Entities;

namespace ProjectManagementService.Application.Interfaces;

/// <summary>
/// Repository cho Reminder - Personal Calendar
/// </summary>
public interface IReminderRepository
{
    System.Threading.Tasks.Task<Reminder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task<List<Reminder>> GetByUserIdAsync(long userId, CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task<List<Reminder>> GetUpcomingRemindersAsync(DateTime fromTime, CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task<List<Reminder>> GetPendingEmailRemindersAsync(CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task<Reminder> CreateAsync(Reminder reminder, CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task<Reminder> UpdateAsync(Reminder reminder, CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task MarkAsEmailSentAsync(Guid id, CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task MarkAsExpiredAsync(List<Guid> ids, CancellationToken cancellationToken = default);
}
