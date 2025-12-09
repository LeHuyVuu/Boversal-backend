using MediatR;
using ProjectManagementService.Application.DTOs.Task;

namespace ProjectManagementService.Application.Features.Tasks.Commands;

/// <summary>
/// Command tạo task mới trong project
/// </summary>
public record CreateTaskCommand(
    long ProjectId,
    long? StatusId,
    string Title,
    string? Description,
    string Priority,
    DateOnly? DueDate,
    int OrderIndex,
    List<long> AssigneeIds
) : IRequest<long>;
