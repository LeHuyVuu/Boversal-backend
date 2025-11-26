using MediatR;

namespace ProjectManagementService.Application.Features.Users.Commands;

/// <summary>
/// Command đổi password
/// </summary>
public class ChangePasswordCommand : IRequest<bool>
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}
