namespace ProjectManagementService.Application.DTOs.User;

/// <summary>
/// DTO cho request cập nhật thông tin profile của user
/// </summary>
public class UpdateUserProfileDto
{
    public string? FullName { get; set; }
    public string? Gender { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
}
