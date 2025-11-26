using Mapster;
using MediatR;
using ProjectManagementService.Application.Common;
using ProjectManagementService.Application.DTOs.Task;
using ProjectManagementService.Application.Interfaces;

namespace ProjectManagementService.Application.Features.Tasks.Queries;

/// <summary>
/// Handler lấy tất cả tasks trong 1 project
/// Dùng cho Kanban board hiển thị tất cả tasks trong project
/// </summary>
public class GetTasksByProjectQueryHandler : IRequestHandler<GetTasksByProjectQuery, PaginatedResponse<TaskDto>>
{
    private readonly ITaskRepository _taskRepository;

    public GetTasksByProjectQueryHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<PaginatedResponse<TaskDto>> Handle(GetTasksByProjectQuery request, CancellationToken cancellationToken)
    {
        var (tasks, totalCount) = await _taskRepository.GetByProjectIdAsync(
            request.ProjectId, 
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
