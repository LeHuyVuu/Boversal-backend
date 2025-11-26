using MediatR;
using ProjectManagementService.Application.Common;
using ProjectManagementService.Application.DTOs;

namespace ProjectManagementService.Application.Features.Projects.Queries;

// Query lấy danh sách project có phân trang
public record GetAllProjectsQuery(
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<PaginatedResponse<ProjectDto>>;
