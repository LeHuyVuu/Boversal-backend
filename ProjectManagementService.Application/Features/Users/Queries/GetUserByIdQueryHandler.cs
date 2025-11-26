using MediatR;
using Mapster;
using ProjectManagementService.Application.DTOs.User;
using ProjectManagementService.Application.Interfaces;
using ProjectManagementService.Domain.Exceptions;

namespace ProjectManagementService.Application.Features.Users.Queries;

/// <summary>
/// Handler lấy thông tin user theo ID
/// </summary>
public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserProfileDto>
{
    private readonly IUserRepository _userRepository;

    public GetUserByIdQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserProfileDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);
        
        if (user == null)
            throw new NotFoundException($"Không tìm thấy user với ID {request.UserId}");

        return user.Adapt<UserProfileDto>();
    }
}
