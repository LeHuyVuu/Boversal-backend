using MediatR;

namespace ProjectManagementService.Application.Features.Tasks.Commands;

/// <summary>
/// Command xóa task
/// </summary>
public record DeleteTaskCommand(long Id) : IRequest<bool>;
