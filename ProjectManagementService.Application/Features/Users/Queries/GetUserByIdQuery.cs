using MediatR;
using ProjectManagementService.Application.DTOs.User;

namespace ProjectManagementService.Application.Features.Users.Queries;

/// <summary>
/// Query lấy thông tin user theo ID
/// </summary>
public class GetUserByIdQuery : IRequest<UserProfileDto>
{
    public long UserId { get; set; }
}
