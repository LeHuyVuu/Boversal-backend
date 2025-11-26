using MediatR;
using ProjectManagementService.Application.Common;
using ProjectManagementService.Application.DTOs.Task;

namespace ProjectManagementService.Application.Features.Tasks.Queries;

/// <summary>
/// Query lấy tasks của current user trong 1 project cụ thể
/// Dùng cho Kanban board khi user chỉ muốn xem tasks của mình
/// </summary>
public record GetMyTasksByProjectQuery(long ProjectId, int PageNumber = 1, int PageSize = 50) 
    : IRequest<PaginatedResponse<TaskDto>>;
