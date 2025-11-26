namespace ProjectManagementService.Application.DTOs.Task;

/// <summary>
/// DTO trả về thông tin Task cho Kanban board
/// </summary>
public class TaskDto
{
    public long Id { get; set; }
    public long ProjectId { get; set; }
    public long? StatusId { get; set; }
    public string StatusName { get; set; }
    public string StatusColor { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Priority { get; set; }
    public DateOnly? DueDate { get; set; }
    public int OrderIndex { get; set; }
    public long CreatedBy { get; set; }
    public string CreatedByName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    /// <summary>
    /// Danh sách users được assign vào task này
    /// </summary>
    public List<TaskAssigneeDto> Assignees { get; set; } = new();
}

public class TaskAssigneeDto
{
    public long UserId { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string AvatarUrl { get; set; }
}
