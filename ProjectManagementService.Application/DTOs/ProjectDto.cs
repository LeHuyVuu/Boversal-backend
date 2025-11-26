namespace ProjectManagementService.Application.DTOs;

public class ProjectDto
{
    public long Id { get; set; }
    public long OwnerId { get; set; }
    public string Name { get; set; }
    public string? DemoUrl { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? ShortIntro { get; set; }
    public string? Highlight { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
