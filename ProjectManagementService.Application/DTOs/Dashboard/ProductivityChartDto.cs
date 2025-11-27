namespace ProjectManagementService.Application.DTOs.Dashboard;

/// <summary>
/// Dữ liệu năng suất làm việc theo thời gian để vẽ chart
/// </summary>
public class ProductivityChartDto
{
    /// <summary>
    /// Thống kê task completion theo tháng (6 tháng gần nhất)
    /// </summary>
    public List<MonthlyProductivityDto> MonthlyCompletion { get; set; } = new();
    
    /// <summary>
    /// Thống kê task theo priority
    /// </summary>
    public TaskByPriorityDto TasksByPriority { get; set; } = new();
    
    /// <summary>
    /// Thống kê task theo status
    /// </summary>
    public List<TaskByStatusDto> TasksByStatus { get; set; } = new();
    
    /// <summary>
    /// Completion rate overview
    /// </summary>
    public CompletionRateDto CompletionRate { get; set; } = new();
}

/// <summary>
/// Năng suất hoàn thành task theo tháng
/// </summary>
public class MonthlyProductivityDto
{
    public string Month { get; set; } = string.Empty; // "Jan", "Feb", "Mar"...
    public int Year { get; set; }
    public int CreatedTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int CompletionRate { get; set; } // Percentage
}

/// <summary>
/// Phân bố task theo priority
/// </summary>
public class TaskByPriorityDto
{
    public int Low { get; set; }
    public int Medium { get; set; }
    public int High { get; set; }
    public int Emergency { get; set; }
}

/// <summary>
/// Phân bố task theo status
/// </summary>
public class TaskByStatusDto
{
    public string StatusName { get; set; } = string.Empty;
    public int Count { get; set; }
    public int Percentage { get; set; } // % so với tổng tasks
}

/// <summary>
/// Tổng quan completion rate
/// </summary>
public class CompletionRateDto
{
    /// <summary>
    /// Tổng số tasks
    /// </summary>
    public int TotalTasks { get; set; }
    
    /// <summary>
    /// Số tasks đã hoàn thành
    /// </summary>
    public int CompletedTasks { get; set; }
    
    /// <summary>
    /// Số tasks đang làm
    /// </summary>
    public int InProgressTasks { get; set; }
    
    /// <summary>
    /// Số tasks chưa bắt đầu
    /// </summary>
    public int TodoTasks { get; set; }
    
    /// <summary>
    /// % hoàn thành
    /// </summary>
    public int CompletionPercentage { get; set; }
    
    /// <summary>
    /// Trung bình số ngày hoàn thành 1 task
    /// </summary>
    public double AverageDaysToComplete { get; set; }
}
