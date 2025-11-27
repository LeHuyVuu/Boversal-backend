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
        // Dùng Mapster map CreateProjectCommand -> Entity
        var project = request.Adapt<Project>();
        
        // Set default values
        project.Status = "active";
        project.CreatedAt = DateTime.UtcNow;
        project.UpdatedAt = DateTime.UtcNow;
        
        // Convert empty strings to null for optional fields
        // This ensures database receives NULL instead of empty string
        if (string.IsNullOrWhiteSpace(project.DemoUrl)) project.DemoUrl = null;
        if (string.IsNullOrWhiteSpace(project.ShortIntro)) project.ShortIntro = null;
        if (string.IsNullOrWhiteSpace(project.Highlight)) project.Highlight = null;
        if (string.IsNullOrWhiteSpace(project.Description)) project.Description = null;

        return await _repository.CreateAsync(project);
    }
}
