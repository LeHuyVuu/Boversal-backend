namespace ProjectManagementService.Application.DTOs.User;

/// <summary>
/// DTO cho kết quả search user (dùng cho assignee dropdown)
/// </summary>
public class SearchUserDto
{
    public long Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
}
