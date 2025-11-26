using MediatR;

namespace ProjectManagementService.Application.Features.Projects.Commands;

public class UpdateProjectCommand : IRequest<bool>
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string? DemoUrl { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? ShortIntro { get; set; }
    public string? Highlight { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; }
}
