using MediatR;
using ProjectManagementService.Application.DTOs;

namespace ProjectManagementService.Application.Features.Projects.Queries;

public class GetProjectByIdQuery : IRequest<ProjectDto?>
{
    public long Id { get; }

    public GetProjectByIdQuery(long id)
    {
        Id = id;
    }
}
