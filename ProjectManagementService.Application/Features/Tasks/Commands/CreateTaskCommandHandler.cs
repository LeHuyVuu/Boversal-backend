using Mapster;
using MediatR;
using ProjectManagementService.Application.Interfaces;
using TaskEntity = ProjectManagementService.Domain.Entities.Task;

namespace ProjectManagementService.Application.Features.Tasks.Commands;

/// <summary>
/// Handler tạo task mới
/// 1. Map CreateTaskCommand -> Task entity bằng Mapster
/// 2. Gán CreatedBy = CurrentUserId
/// 3. Lưu task vào database
/// 4. Thêm assignees vào task
/// 5. Trả về TaskId
/// </summary>
public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, long>
{
    private readonly ITaskRepository _taskRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateTaskCommandHandler(ITaskRepository taskRepository, ICurrentUserService currentUserService)
    {
        _taskRepository = taskRepository;
        _currentUserService = currentUserService;
    }

    public async Task<long> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        // Dùng Mapster map CreateTaskCommand -> Task entity
        var task = request.Adapt<TaskEntity>();
        
        // Gán CreatedBy = CurrentUserId
        task.CreatedBy = _currentUserService.UserId ?? 0;
        task.CreatedAt = DateTime.UtcNow;
        task.UpdatedAt = DateTime.UtcNow;

        // Lưu task vào database
        var createdTask = await _taskRepository.AddAsync(task);

        // Thêm assignees vào task
        if (request.AssigneeIds != null && request.AssigneeIds.Any())
        {
            foreach (var userId in request.AssigneeIds)
            {
                await _taskRepository.AddAssigneeAsync(createdTask.Id, userId);
            }
        }

        return createdTask.Id;
    }
}
