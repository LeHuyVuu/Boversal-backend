using Mapster;
using MediatR;
using ProjectManagementService.Application.DTOs.Task;
using ProjectManagementService.Application.Interfaces;

namespace ProjectManagementService.Application.Features.Tasks.Queries;

/// <summary>
/// Handler lấy task theo ID
/// Map Task entity -> TaskDto bằng Mapster
/// </summary>
public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, TaskDto?>
{
    private readonly ITaskRepository _taskRepository;

    public GetTaskByIdQueryHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<TaskDto?> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(request.Id);
        
        // Dùng Mapster map Task entity -> TaskDto
        return task?.Adapt<TaskDto>();
    }
}
