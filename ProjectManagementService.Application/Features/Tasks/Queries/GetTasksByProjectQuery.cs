using MediatR;
using ProjectManagementService.Application.Common;
using ProjectManagementService.Application.DTOs.Task;

namespace ProjectManagementService.Application.Features.Tasks.Queries;

/// <summary>
/// Query lấy tất cả tasks trong 1 project (có phân trang)
/// </summary>
public record GetTasksByProjectQuery(long ProjectId, int PageNumber = 1, int PageSize = 50) 
    : IRequest<PaginatedResponse<TaskDto>>;
