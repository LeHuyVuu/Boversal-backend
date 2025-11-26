using Mapster;
using MediatR;
using ProjectManagementService.Application.DTOs.Task;
using ProjectManagementService.Application.Interfaces;
using ProjectManagementService.Domain.Exceptions;

namespace ProjectManagementService.Application.Features.Tasks.Commands;

/// <summary>
/// Handler PATCH task - chỉ cập nhật 1 vài fields
/// Dùng cho Kanban khi kéo task:
/// - Sang column khác: cập nhật StatusId
/// - Thay đổi vị trí: cập nhật OrderIndex
/// - Thay đổi priority: cập nhật Priority
/// </summary>
public class PatchTaskCommandHandler : IRequestHandler<PatchTaskCommand, bool>
{
    private readonly ITaskRepository _taskRepository;

    public PatchTaskCommandHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<bool> Handle(PatchTaskCommand request, CancellationToken cancellationToken)
    {
        // Check task có tồn tại không
        var existingTask = await _taskRepository.GetByIdAsync(request.Id);
        if (existingTask == null)
            throw new NotFoundException($"Không tìm thấy task với id {request.Id}");

        // Map PatchTaskCommand -> PatchTaskDto
        var patchDto = request.Adapt<PatchTaskDto>();

        // Cập nhật chỉ những fields không null
        return await _taskRepository.PatchAsync(request.Id, patchDto);
    }
}
