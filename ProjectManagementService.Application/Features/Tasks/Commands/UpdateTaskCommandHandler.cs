using Mapster;
using MediatR;
using ProjectManagementService.Application.Interfaces;
using ProjectManagementService.Domain.Exceptions;

namespace ProjectManagementService.Application.Features.Tasks.Commands;

/// <summary>
/// Handler cập nhật task
/// 1. Lấy task từ database
/// 2. Map UpdateTaskCommand -> Task entity bằng Mapster
/// 3. Cập nhật UpdatedAt
/// 4. Lưu vào database
/// 5. Cập nhật assignees
/// 6. Trả về true/false
/// </summary>
public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, bool>
{
    private readonly ITaskRepository _taskRepository;

    public UpdateTaskCommandHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<bool> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        // Lấy task hiện tại từ database
        var existingTask = await _taskRepository.GetByIdAsync(request.Id);
        if (existingTask == null)
            throw new NotFoundException($"Không tìm thấy task với id {request.Id}");

        // Dùng Mapster map UpdateTaskCommand -> Task entity
        request.Adapt(existingTask);
        existingTask.UpdatedAt = DateTime.UtcNow;

        // Cập nhật task
        var result = await _taskRepository.UpdateAsync(existingTask);

        if (result)
        {
            // Xóa tất cả assignees cũ
            await _taskRepository.RemoveAllAssigneesAsync(request.Id);

            // Thêm assignees mới
            if (request.AssigneeIds != null && request.AssigneeIds.Any())
            {
                foreach (var userId in request.AssigneeIds)
                {
                    await _taskRepository.AddAssigneeAsync(request.Id, userId);
                }
            }
        }

        return result;
    }
}
