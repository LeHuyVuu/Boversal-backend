namespace ProjectManagementService.Application.DTOs.Task;

public class UpdateTaskDto
{
    public long Id { get; set; }
    public long ProjectId { get; set; }
    public long? StatusId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Priority { get; set; }
    public DateOnly? DueDate { get; set; }
    public int OrderIndex { get; set; }
    
    /// <summary>
    /// Danh sách UserId được assign vào task
    /// </summary>
    public List<long> AssigneeIds { get; set; } = new();
}
