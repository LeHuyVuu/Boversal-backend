using MediatR;
using ProjectManagementService.Application.DTOs.Dashboard;
using ProjectManagementService.Application.Interfaces;

namespace ProjectManagementService.Application.Features.Dashboard.Queries;

public class GetDashboardDataQueryHandler : IRequestHandler<GetDashboardDataQuery, DashboardDataDto>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ITaskRepository _taskRepository;
    private readonly IProjectRepository _projectRepository;

    public GetDashboardDataQueryHandler(
        ICurrentUserService currentUserService,
        ITaskRepository taskRepository,
        IProjectRepository projectRepository)
    {
        _currentUserService = currentUserService;
        _taskRepository = taskRepository;
        _projectRepository = projectRepository;
    }

    public async Task<DashboardDataDto> Handle(GetDashboardDataQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId 
            ?? throw new UnauthorizedAccessException("User not authenticated");

        // Lấy tất cả tasks và projects của user
        var userTasks = await _taskRepository.GetUserTasksAsync(currentUserId);
        var userProjects = await _projectRepository.GetUserProjectsAsync(currentUserId);

        // Calculate stats
        var now = DateTime.UtcNow;
        var thisMonthStart = new DateTime(now.Year, now.Month, 1);
        var lastMonthStart = thisMonthStart.AddMonths(-1);

        var totalTasks = userTasks.Count;
        var activeProjects = userProjects.Count(p => p.Status == "active" || string.IsNullOrEmpty(p.Status));
        var openIssues = userTasks.Count(t => t.Status?.Name != "Completed" && t.Status?.Name != "Done");

        // Calculate month-over-month changes
        var tasksThisMonth = userTasks.Count(t => t.CreatedAt >= thisMonthStart);
        var tasksLastMonth = userTasks.Count(t => t.CreatedAt >= lastMonthStart && t.CreatedAt < thisMonthStart);
        var taskChange = tasksThisMonth - tasksLastMonth;

        var projectsThisMonth = userProjects.Count(p => p.CreatedAt >= thisMonthStart);
        var projectsLastMonth = userProjects.Count(p => p.CreatedAt >= lastMonthStart && p.CreatedAt < thisMonthStart);
        var projectChange = projectsThisMonth - projectsLastMonth;

        // Get recent tasks (top 5)
        var recentTasks = userTasks
            .OrderByDescending(t => t.CreatedAt)
            .Take(5)
            .Select(t => new RecentTaskDto
            {
                Id = t.Id,
                Title = t.Title,
                ProjectCode = $"PRJ-{t.ProjectId}",
                StatusName = t.Status?.Name ?? "No Status",
                Priority = t.Priority,
                DueDate = t.DueDate.HasValue ? t.DueDate.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null,
                CreatedAt = t.CreatedAt
            })
            .ToList();

        // Get active projects with progress
        var activeProjectsData = userProjects
            .Where(p => p.Status == "active" || string.IsNullOrEmpty(p.Status))
            .Select(p =>
            {
                var projectTasks = p.Tasks.Where(t => t.DeletedAt == null).ToList();
                var totalTasksCount = projectTasks.Count;
                var completedTasksCount = projectTasks.Count(t => 
                    t.Status?.Name == "Completed" || t.Status?.Name == "Done");
                var progressPercentage = totalTasksCount > 0 
                    ? (int)Math.Round((double)completedTasksCount / totalTasksCount * 100) 
                    : 0;

                return new ProjectProgressDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Code = $"PRJ-{p.Id}",
                    Description = p.Description,
                    TotalTasks = totalTasksCount,
                    CompletedTasks = completedTasksCount,
                    ProgressPercentage = progressPercentage
                };
            })
            .OrderByDescending(p => p.TotalTasks)
            .ToList();

        return new DashboardDataDto
        {
            Stats = new DashboardStatsDto
            {
                TotalTasks = totalTasks,
                ActiveProjects = activeProjects,
                OpenIssues = openIssues,
                TeamMembers = 1, // Personal app - always 1
                TaskChangeVsLastMonth = taskChange,
                ProjectChangeVsLastMonth = projectChange,
                IssueChangeVsLastMonth = 0
            },
            RecentTasks = recentTasks,
            ActiveProjects = activeProjectsData
        };
    }
}
