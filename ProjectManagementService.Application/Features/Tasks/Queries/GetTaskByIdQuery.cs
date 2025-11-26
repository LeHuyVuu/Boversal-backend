using MediatR;
using ProjectManagementService.Application.DTOs.Task;

namespace ProjectManagementService.Application.Features.Tasks.Queries;

/// <summary>
/// Query lấy task theo ID
/// </summary>
public record GetTaskByIdQuery(long Id) : IRequest<TaskDto?>;
