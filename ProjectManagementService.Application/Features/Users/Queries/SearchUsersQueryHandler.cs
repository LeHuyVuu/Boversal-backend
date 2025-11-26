using MediatR;
using Mapster;
using Microsoft.EntityFrameworkCore;
using ProjectManagementService.Application.DTOs.User;
using ProjectManagementService.Application.Interfaces;

namespace ProjectManagementService.Application.Features.Users.Queries;

/// <summary>
/// Handler tìm kiếm user theo tên hoặc email
/// </summary>
public class SearchUsersQueryHandler : IRequestHandler<SearchUsersQuery, List<SearchUserDto>>
{
    private readonly IUserRepository _userRepository;

    public SearchUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<List<SearchUserDto>> Handle(SearchUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.SearchUsersAsync(request.SearchTerm, request.Limit);
        return users.Adapt<List<SearchUserDto>>();
    }
}
