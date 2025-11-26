using Mapster;
using MediatR;
using ProjectManagementService.Application.Common;
using ProjectManagementService.Application.DTOs;
using ProjectManagementService.Application.Interfaces;

namespace ProjectManagementService.Application.Features.Projects.Queries;

public class GetAllProjectsQueryHandler : IRequestHandler<GetAllProjectsQuery, PaginatedResponse<ProjectDto>>
{
    private readonly IProjectRepository _repository;
    private readonly ICurrentUserService _currentUserService;

    public GetAllProjectsQueryHandler(IProjectRepository repository, ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    public async Task<PaginatedResponse<ProjectDto>> Handle(GetAllProjectsQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId 
            ?? throw new UnauthorizedAccessException("User not authenticated");

        // Lấy tất cả projects của user hiện tại
        var userProjects = await _repository.GetUserProjectsAsync(currentUserId);
        
        var totalRecords = userProjects.Count;

        // Phân trang thủ công
        var pagedProjects = userProjects
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();
        
        // Dùng Mapster để map sang DTO
        var projectDtos = pagedProjects.Adapt<List<ProjectDto>>();

        return PaginatedResponse<ProjectDto>.Create(
            projectDtos,
            request.PageNumber,
            request.PageSize,
            totalRecords
        );
    }
}
