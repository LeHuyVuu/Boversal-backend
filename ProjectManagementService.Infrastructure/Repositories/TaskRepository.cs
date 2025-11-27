using Microsoft.EntityFrameworkCore;
using ProjectManagementService.Application.DTOs.Task;
using ProjectManagementService.Application.Interfaces;
using ProjectManagementService.Domain.Entities;
using ProjectManagementService.Infrastructure.Persistence;
using TaskEntity = ProjectManagementService.Domain.Entities.Task;

namespace ProjectManagementService.Infrastructure.Repositories;

/// <summary>
/// Repository implementation cho Task
/// </summary>
public class TaskRepository : ITaskRepository
{
    private readonly MyDbContext _context;

    public TaskRepository(MyDbContext context)
    {
        _context = context;
    }

    public async System.Threading.Tasks.Task<TaskEntity?> GetByIdAsync(long id)
    {
        return await _context.Tasks
            .Include(t => t.Status)
            .Include(t => t.CreatedByNavigation)
            .Include(t => t.TaskAssignees)
                .ThenInclude(ta => ta.User)
            .FirstOrDefaultAsync(t => t.Id == id && t.DeletedAt == null);
    }

    public async System.Threading.Tasks.Task<(List<TaskEntity> Tasks, int TotalCount)> GetByProjectIdAsync(long projectId, int pageNumber, int pageSize)
    {
        var query = _context.Tasks
            .Include(t => t.Status)
            .Include(t => t.CreatedByNavigation)
            .Include(t => t.TaskAssignees)
                .ThenInclude(ta => ta.User)
            .Where(t => t.ProjectId == projectId && t.DeletedAt == null)
            .OrderBy(t => t.OrderIndex)
            .ThenByDescending(t => t.CreatedAt);

        var totalCount = await query.CountAsync();
        var tasks = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (tasks, totalCount);
    }

    public async System.Threading.Tasks.Task<(List<TaskEntity> Tasks, int TotalCount)> GetByProjectAndUserAsync(long projectId, long userId, int pageNumber, int pageSize)
    {
        var query = _context.Tasks
            .Include(t => t.Status)
            .Include(t => t.CreatedByNavigation)
            .Include(t => t.TaskAssignees)
                .ThenInclude(ta => ta.User)
            .Where(t => t.ProjectId == projectId 
                && t.DeletedAt == null
                && t.TaskAssignees.Any(ta => ta.UserId == userId && ta.DeletedAt == null))
            .OrderBy(t => t.OrderIndex)
            .ThenByDescending(t => t.CreatedAt);

        var totalCount = await query.CountAsync();
        var tasks = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (tasks, totalCount);
    }

    public async System.Threading.Tasks.Task<(List<TaskEntity> Tasks, int TotalCount)> GetByUserIdAsync(long userId, int pageNumber, int pageSize)
    {
        var query = _context.Tasks
            .Include(t => t.Status)
            .Include(t => t.CreatedByNavigation)
            .Include(t => t.TaskAssignees)
                .ThenInclude(ta => ta.User)
            .Where(t => t.DeletedAt == null
                && t.TaskAssignees.Any(ta => ta.UserId == userId && ta.DeletedAt == null))
            .OrderByDescending(t => t.CreatedAt);

        var totalCount = await query.CountAsync();
        var tasks = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (tasks, totalCount);
    }

    public async System.Threading.Tasks.Task<TaskEntity> AddAsync(TaskEntity task)
    {
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async System.Threading.Tasks.Task<bool> UpdateAsync(TaskEntity task)
    {
        _context.Tasks.Update(task);
        return await _context.SaveChangesAsync() > 0;
    }

    public async System.Threading.Tasks.Task<bool> DeleteAsync(long id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null) return false;

        // Soft delete
        task.DeletedAt = DateTime.UtcNow;
        return await _context.SaveChangesAsync() > 0;
    }

    public async System.Threading.Tasks.Task<bool> PatchAsync(long id, PatchTaskDto patchDto)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null) return false;

        // Chỉ cập nhật các fields không null
        if (patchDto.StatusId.HasValue)
            task.StatusId = patchDto.StatusId.Value;

        if (patchDto.OrderIndex.HasValue)
            task.OrderIndex = patchDto.OrderIndex.Value;

        if (!string.IsNullOrEmpty(patchDto.Priority))
            task.Priority = patchDto.Priority;

        task.UpdatedAt = DateTime.UtcNow;

        return await _context.SaveChangesAsync() > 0;
    }

    public async System.Threading.Tasks.Task AddAssigneeAsync(long taskId, long userId)
    {
        var assignee = new TaskAssignee
        {
            TaskId = taskId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.TaskAssignees.Add(assignee);
        await _context.SaveChangesAsync();
    }

    public async System.Threading.Tasks.Task RemoveAllAssigneesAsync(long taskId)
    {
        var assignees = await _context.TaskAssignees
            .Where(ta => ta.TaskId == taskId)
            .ToListAsync();

        _context.TaskAssignees.RemoveRange(assignees);
        await _context.SaveChangesAsync();
    }

    public async System.Threading.Tasks.Task<List<TaskEntity>> GetUserTasksAsync(long userId)
    {
        return await _context.Tasks
            .Include(t => t.Status)
            .Include(t => t.Project)
            .Include(t => t.TaskAssignees)
                .ThenInclude(ta => ta.User)
            .Where(t => t.DeletedAt == null
                && t.Project.DeletedAt == null  // Chỉ lấy tasks của projects chưa bị xóa
                && (
                    // Tasks được assign cho user
                    t.TaskAssignees.Any(ta => ta.UserId == userId && ta.DeletedAt == null)
                    // HOẶC tasks trong projects mà user là owner
                    || t.Project.OwnerId == userId
                    // HOẶC tasks được tạo bởi user
                    || t.CreatedBy == userId
                ))
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }
}
