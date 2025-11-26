using MediatR;
using ProjectManagementService.Application.DTOs.Task;

namespace ProjectManagementService.Application.Features.Tasks.Commands;

/// <summary>
/// Command PATCH task - chỉ cập nhật 1 vài fields (cho Kanban drag & drop)
/// Khi kéo task trong Kanban, chỉ cần update StatusId và OrderIndex
/// </summary>
public record PatchTaskCommand(
    long Id,
    long? StatusId,
    int? OrderIndex,
    string? Priority
) : IRequest<bool>;
