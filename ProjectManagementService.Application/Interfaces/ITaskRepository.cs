using ProjectManagementService.Application.DTOs.Task;
using ProjectManagementService.Domain.Entities;
using TaskEntity = ProjectManagementService.Domain.Entities.Task;

namespace ProjectManagementService.Application.Interfaces;

/// <summary>
/// Repository cho Task entity
/// </summary>
public interface ITaskRepository
{
    /// <summary>
    /// Lấy task theo ID (bao gồm Status, Assignees, CreatedBy)
    /// </summary>
    System.Threading.Tasks.Task<TaskEntity?> GetByIdAsync(long id);
    
    /// <summary>
    /// Lấy tất cả tasks trong 1 project (có phân trang)
    /// </summary>
    System.Threading.Tasks.Task<(List<TaskEntity> Tasks, int TotalCount)> GetByProjectIdAsync(long projectId, int pageNumber, int pageSize);
    
    /// <summary>
    /// Lấy tasks của current user trong 1 project cụ thể
    /// </summary>
    System.Threading.Tasks.Task<(List<TaskEntity> Tasks, int TotalCount)> GetByProjectAndUserAsync(long projectId, long userId, int pageNumber, int pageSize);
    
    /// <summary>
    /// Lấy tất cả tasks của 1 user (trong tất cả projects)
    /// </summary>
    System.Threading.Tasks.Task<(List<TaskEntity> Tasks, int TotalCount)> GetByUserIdAsync(long userId, int pageNumber, int pageSize);
    
    /// <summary>
    /// Tạo task mới
    /// </summary>
    System.Threading.Tasks.Task<TaskEntity> AddAsync(TaskEntity task);
    
    /// <summary>
    /// Cập nhật task
    /// </summary>
    System.Threading.Tasks.Task<bool> UpdateAsync(TaskEntity task);
    
    /// <summary>
    /// Xóa task (soft delete)
    /// </summary>
    System.Threading.Tasks.Task<bool> DeleteAsync(long id);
    
    /// <summary>
    /// Patch task - chỉ cập nhật 1 vài fields (cho Kanban drag & drop)
    /// </summary>
    System.Threading.Tasks.Task<bool> PatchAsync(long id, PatchTaskDto patchDto);
    
    /// <summary>
    /// Thêm assignee vào task
    /// </summary>
    System.Threading.Tasks.Task AddAssigneeAsync(long taskId, long userId);
    
    /// <summary>
    /// Xóa tất cả assignees của task
    /// </summary>
    System.Threading.Tasks.Task RemoveAllAssigneesAsync(long taskId);
    
    /// <summary>
    /// Lấy tất cả tasks của user (cho Dashboard)
    /// </summary>
    System.Threading.Tasks.Task<List<TaskEntity>> GetUserTasksAsync(long userId);
}
