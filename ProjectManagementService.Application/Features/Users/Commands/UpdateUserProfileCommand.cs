using MediatR;
using ProjectManagementService.Application.DTOs.User;

namespace ProjectManagementService.Application.Features.Users.Commands;

/// <summary>
/// Command cập nhật thông tin profile của user hiện tại
/// </summary>
public class UpdateUserProfileCommand : IRequest<UserProfileDto>
{
    public string? FullName { get; set; }
    public string? Gender { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
}
