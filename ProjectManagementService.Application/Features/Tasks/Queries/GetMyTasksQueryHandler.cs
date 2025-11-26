using Mapster;
using MediatR;
using ProjectManagementService.Application.Common;
using ProjectManagementService.Application.DTOs.Task;
using ProjectManagementService.Application.Interfaces;
using ProjectManagementService.Domain.Exceptions;

namespace ProjectManagementService.Application.Features.Tasks.Queries;

/// <summary>
/// Handler lấy tất cả tasks của current user (trong tất cả projects)
/// Current user được lấy từ JWT token qua ICurrentUserService
/// </summary>
public class GetMyTasksQueryHandler : IRequestHandler<GetMyTasksQuery, PaginatedResponse<TaskDto>>
{
    private readonly ITaskRepository _taskRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetMyTasksQueryHandler(ITaskRepository taskRepository, ICurrentUserService currentUserService)
    {
        _taskRepository = taskRepository;
        _currentUserService = currentUserService;
    }

    public async Task<PaginatedResponse<TaskDto>> Handle(GetMyTasksQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
            throw new UnauthorizedAccessException("User chưa đăng nhập");

        var (tasks, totalCount) = await _taskRepository.GetByUserIdAsync(
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
