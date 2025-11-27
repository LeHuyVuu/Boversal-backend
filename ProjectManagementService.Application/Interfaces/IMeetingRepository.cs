using ProjectManagementService.Application.DTOs.Meeting;

namespace ProjectManagementService.Application.Interfaces;

/// <summary>
/// Repository interface cho Meeting
/// </summary>
public interface IMeetingRepository
{
    Task<Domain.Entities.Meeting?> GetByIdAsync(long id);
    
    Task<List<Domain.Entities.Meeting>> GetUserMeetingsAsync(long userId);
    
    Task<Domain.Entities.Meeting> CreateAsync(Domain.Entities.Meeting meeting);
    
    Task<bool> UpdateAsync(Domain.Entities.Meeting meeting);
    
    Task<bool> DeleteAsync(long id);
}
