using MediatR;

namespace ProjectManagementService.Application.Features.Tasks.Commands;

/// <summary>
/// Command cập nhật task
/// </summary>
public record UpdateTaskCommand(
    long Id,
    long ProjectId,
    long? StatusId,
    string Title,
    string Description,
    string Priority,
    DateOnly? DueDate,
    int OrderIndex,
    List<long> AssigneeIds
) : IRequest<bool>;
