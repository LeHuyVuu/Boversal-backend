namespace ProjectManagementService.Application.DTOs.Task;

/// <summary>
/// DTO cho PATCH - chỉ cập nhật 1 vài fields (dùng cho Kanban drag & drop)
/// </summary>
public class PatchTaskDto
{
    public long Id { get; set; }
    
    /// <summary>
    /// Cập nhật StatusId khi kéo task sang column khác trong Kanban
    /// </summary>
    public long? StatusId { get; set; }
    
    /// <summary>
    /// Cập nhật OrderIndex khi kéo task thay đổi vị trí
    /// </summary>
    public int? OrderIndex { get; set; }
    
    /// <summary>
    /// Cập nhật Priority (optional: "low", "medium", "high", "emergency")
    /// </summary>
    public string? Priority { get; set; }
}
