using Mapster;
using MediatR;
using ProjectManagementService.Application.DTOs;
using ProjectManagementService.Application.Interfaces;

namespace ProjectManagementService.Application.Features.Projects.Queries;

public class GetProjectByIdQueryHandler : IRequestHandler<GetProjectByIdQuery, ProjectDto?>
{
    private readonly IProjectRepository _repository;

    public GetProjectByIdQueryHandler(IProjectRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProjectDto?> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var project = await _repository.GetByIdAsync(request.Id);
        
        if (project == null)
            return null;

        // Dùng Mapster để map sang DTO
        return project.Adapt<ProjectDto>();
    }
}
