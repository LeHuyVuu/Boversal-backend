using Microsoft.EntityFrameworkCore;
using ProjectManagementService.Application.Interfaces;
using ProjectManagementService.Domain.Entities;
using ProjectManagementService.Infrastructure.Persistence;

namespace ProjectManagementService.Infrastructure.Repositories;

/// <summary>
/// Repository implementation cho Reminder
/// </summary>
public class ReminderRepository : IReminderRepository
{
    private readonly MyDbContext _context;

    public ReminderRepository(MyDbContext context)
    {
        _context = context;
    }

    public async Task<Reminder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Reminders
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async System.Threading.Tasks.Task<List<Reminder>> GetByUserIdAsync(long userId, CancellationToken cancellationToken = default)
    {
        return await _context.Reminders
            .Where(r => r.UserId == userId)
            .OrderBy(r => r.ReminderTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Reminder>> GetUpcomingRemindersAsync(DateTime fromTime, CancellationToken cancellationToken = default)
    {
        return await _context.Reminders
            .Where(r => r.ReminderTime >= fromTime && !r.IsExpired && !r.IsCompleted)
            .OrderBy(r => r.ReminderTime)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Lấy danh sách reminder cần gửi email (trước 15 phút, chưa gửi email)
    /// </summary>
    public async Task<List<Reminder>> GetPendingEmailRemindersAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var reminderWindow = now.AddMinutes(15); // Gửi email trước 15 phút

        return await _context.Reminders
            .Include(r => r.User)
            .Where(r => !r.IsEmailSent 
                     && !r.IsExpired 
                     && !r.IsCompleted
                     && r.ReminderTime <= reminderWindow
                     && r.ReminderTime > now)
            .ToListAsync(cancellationToken);
    }

    public async Task<Reminder> CreateAsync(Reminder reminder, CancellationToken cancellationToken = default)
    {
        reminder.Id = Guid.NewGuid();
        reminder.CreatedAt = DateTime.UtcNow;
        
        await _context.Reminders.AddAsync(reminder, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        
        return reminder;
    }

    public async Task<Reminder> UpdateAsync(Reminder reminder, CancellationToken cancellationToken = default)
    {
        reminder.UpdatedAt = DateTime.UtcNow;
        
        _context.Reminders.Update(reminder);
        await _context.SaveChangesAsync(cancellationToken);
        
        return reminder;
    }

    public async System.Threading.Tasks.Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var reminder = await _context.Reminders.FindAsync(new object[] { id }, cancellationToken);
        if (reminder != null)
        {
            _context.Reminders.Remove(reminder);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async System.Threading.Tasks.Task MarkAsEmailSentAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var reminder = await _context.Reminders.FindAsync(new object[] { id }, cancellationToken);
        if (reminder != null)
        {
            reminder.IsEmailSent = true;
            reminder.EmailSentAt = DateTime.UtcNow;
            reminder.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async System.Threading.Tasks.Task MarkAsExpiredAsync(List<Guid> ids, CancellationToken cancellationToken = default)
    {
        var reminders = await _context.Reminders
            .Where(r => ids.Contains(r.Id))
            .ToListAsync(cancellationToken);

        foreach (var reminder in reminders)
        {
            reminder.IsExpired = true;
            reminder.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
