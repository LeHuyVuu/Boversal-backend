using MediatR;

namespace ProjectManagementService.Application.Features.Projects.Commands;

public class DeleteProjectCommand : IRequest<bool>
{
    public long Id { get; }

    public DeleteProjectCommand(long id)
    {
        Id = id;
    }
}
