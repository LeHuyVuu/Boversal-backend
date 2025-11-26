namespace ProjectManagementService.Application.DTOs.Dashboard;

public class ProjectProgressDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int ProgressPercentage { get; set; }
}
