using MediatR;
using ProjectManagementService.Application.DTOs.User;

namespace ProjectManagementService.Application.Features.Users.Queries;

/// <summary>
/// Query tìm kiếm user theo tên hoặc email (dùng cho assignee dropdown)
/// </summary>
public class SearchUsersQuery : IRequest<List<SearchUserDto>>
{
    public string? SearchTerm { get; set; }
    public int Limit { get; set; } = 10;
}
