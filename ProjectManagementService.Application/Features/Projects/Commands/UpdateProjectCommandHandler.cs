using Mapster;
using MediatR;
using ProjectManagementService.Application.Interfaces;

namespace ProjectManagementService.Application.Features.Projects.Commands;

public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, bool>
{
    private readonly IProjectRepository _repository;

    public UpdateProjectCommandHandler(IProjectRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _repository.GetByIdAsync(request.Id);
        if (project == null)
            return false;

        // Dùng Mapster để map Command vào Entity (chỉ update fields không null)
        request.Adapt(project);
        project.UpdatedAt = DateTime.UtcNow;

        return await _repository.UpdateAsync(project);
    }
}
