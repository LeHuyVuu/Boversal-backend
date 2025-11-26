namespace ProjectManagementService.Application.DTOs.Dashboard;

public class DashboardStatsDto
{
    public int TotalTasks { get; set; }
    public int ActiveProjects { get; set; }
    public int OpenIssues { get; set; }
    public int TeamMembers { get; set; } = 1; // For personal app, always 1
    
    public int TaskChangeVsLastMonth { get; set; }
    public int ProjectChangeVsLastMonth { get; set; }
    public int IssueChangeVsLastMonth { get; set; }
}
