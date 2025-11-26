using MediatR;
using ProjectManagementService.Application.DTOs;

namespace ProjectManagementService.Application.Features.Projects.Commands;

public class CreateProjectCommand : IRequest<long>
{
    public long OwnerId { get; set; }
    public string Name { get; set; }
    public string? DemoUrl { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? ShortIntro { get; set; }
    public string? Highlight { get; set; }
    public string? Description { get; set; }
}
