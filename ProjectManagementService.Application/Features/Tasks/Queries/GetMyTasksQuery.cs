using MediatR;
using ProjectManagementService.Application.Common;
using ProjectManagementService.Application.DTOs.Task;

namespace ProjectManagementService.Application.Features.Tasks.Queries;

/// <summary>
/// Query lấy tasks của current user đang login trong tất cả projects
/// </summary>
public record GetMyTasksQuery(int PageNumber = 1, int PageSize = 50) 
    : IRequest<PaginatedResponse<TaskDto>>;
