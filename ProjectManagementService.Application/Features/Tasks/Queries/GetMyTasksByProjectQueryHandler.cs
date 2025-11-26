using Mapster;
using MediatR;
using ProjectManagementService.Application.Common;
using ProjectManagementService.Application.DTOs.Task;
using ProjectManagementService.Application.Interfaces;

namespace ProjectManagementService.Application.Features.Tasks.Queries;

/// <summary>
/// Handler lấy tasks của current user trong 1 project cụ thể
/// Dùng cho Kanban board khi user filter "My Tasks"
/// </summary>
public class GetMyTasksByProjectQueryHandler : IRequestHandler<GetMyTasksByProjectQuery, PaginatedResponse<TaskDto>>
{
    private readonly ITaskRepository _taskRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetMyTasksByProjectQueryHandler(ITaskRepository taskRepository, ICurrentUserService currentUserService)
    {
        _taskRepository = taskRepository;
        _currentUserService = currentUserService;
    }

    public async Task<PaginatedResponse<TaskDto>> Handle(GetMyTasksByProjectQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
            throw new UnauthorizedAccessException("User chưa đăng nhập");

        var (tasks, totalCount) = await _taskRepository.GetByProjectAndUserAsync(
            request.ProjectId,
            userId.Value, 
            request.PageNumber, 
            request.PageSize);

        // Dùng Mapster map List<Task> -> List<TaskDto>
        var taskDtos = tasks.Adapt<List<TaskDto>>();

        return PaginatedResponse<TaskDto>.Create(
            taskDtos,
            request.PageNumber,
            request.PageSize,
            totalCount
        );
    }
}
