using Microsoft.EntityFrameworkCore;
using ProjectManagementService.Application.Interfaces;
using ProjectManagementService.Domain.Entities;
using ProjectManagementService.Infrastructure.Persistence;

namespace ProjectManagementService.Infrastructure.Repositories;

/// <summary>
/// Repository implementation cho Meeting
/// </summary>
public class MeetingRepository : IMeetingRepository
{
    private readonly MyDbContext _context;

    public MeetingRepository(MyDbContext context)
    {
        _context = context;
    }

    public async Task<Meeting?> GetByIdAsync(long id)
    {
        return await _context.Set<Meeting>()
            .FirstOrDefaultAsync(m => m.Id == id && m.DeletedAt == null);
    }

    public async Task<List<Meeting>> GetUserMeetingsAsync(long userId)
    {
        return await _context.Set<Meeting>()
            .Where(m => m.UserId == userId && m.DeletedAt == null)
            .OrderBy(m => m.StartTime)
            .ToListAsync();
    }

    public async Task<Meeting> CreateAsync(Meeting meeting)
    {
        meeting.CreatedAt = DateTime.UtcNow;
        meeting.UpdatedAt = DateTime.UtcNow;
        
        _context.Set<Meeting>().Add(meeting);
        await _context.SaveChangesAsync();
        
        return meeting;
    }

    public async Task<bool> UpdateAsync(Meeting meeting)
    {
        meeting.UpdatedAt = DateTime.UtcNow;
        
        _context.Set<Meeting>().Update(meeting);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var meeting = await GetByIdAsync(id);
        if (meeting == null) return false;

        meeting.DeletedAt = DateTime.UtcNow;
        return await _context.SaveChangesAsync() > 0;
    }
}
