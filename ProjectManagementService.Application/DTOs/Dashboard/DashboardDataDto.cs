namespace ProjectManagementService.Application.DTOs.Dashboard;

public class DashboardDataDto
{
    public DashboardStatsDto Stats { get; set; } = new();
    public List<RecentTaskDto> RecentTasks { get; set; } = new();
    public List<ProjectProgressDto> ActiveProjects { get; set; } = new();
}
