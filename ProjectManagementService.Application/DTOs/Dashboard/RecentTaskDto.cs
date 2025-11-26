namespace ProjectManagementService.Application.DTOs.Dashboard;

public class RecentTaskDto
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ProjectCode { get; set; } = string.Empty;
    public string StatusName { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; }
}
