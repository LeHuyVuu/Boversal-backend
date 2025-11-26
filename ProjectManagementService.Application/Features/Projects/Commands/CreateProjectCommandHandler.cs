using Mapster;
using MediatR;
using ProjectManagementService.Application.Interfaces;
using ProjectManagementService.Domain.Entities;

namespace ProjectManagementService.Application.Features.Projects.Commands;

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, long>
{
    private readonly IProjectRepository _repository;

    public CreateProjectCommandHandler(IProjectRepository repository)
    {
        _repository = repository;
    }

    public async Task<long> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        // Dùng Mapster để map Command sang Entity
        var project = request.Adapt<Project>();
        project.Status = "active";
        project.CreatedAt = DateTime.UtcNow;
        project.UpdatedAt = DateTime.UtcNow;

        return await _repository.CreateAsync(project);
    }
}
