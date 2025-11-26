using ProjectManagementService.Domain.Entities;

namespace ProjectManagementService.Application.Interfaces;

public interface IProjectRepository
{
    Task<List<Project>> GetAllAsync();
    Task<List<Project>> GetPagedAsync(int pageNumber, int pageSize);
    Task<int> CountAsync();
    Task<Project?> GetByIdAsync(long id);
    Task<long> CreateAsync(Project project);
    Task<bool> UpdateAsync(Project project);
    Task<bool> DeleteAsync(long id);
    
    /// <summary>
    /// Lấy tất cả projects của user (cho Dashboard)
    /// </summary>
    Task<List<Project>> GetUserProjectsAsync(long userId);
}
