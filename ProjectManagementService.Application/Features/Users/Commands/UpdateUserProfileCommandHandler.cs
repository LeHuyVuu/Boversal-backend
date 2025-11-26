using MediatR;
using Mapster;
using ProjectManagementService.Application.DTOs.User;
using ProjectManagementService.Application.Interfaces;
using ProjectManagementService.Domain.Exceptions;

namespace ProjectManagementService.Application.Features.Users.Commands;

/// <summary>
/// Handler cập nhật thông tin profile của user hiện tại
/// </summary>
public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, UserProfileDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public UpdateUserProfileCommandHandler(
        IUserRepository userRepository,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<UserProfileDto> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        // Lấy UserId từ JWT token
        var userId = _currentUserService.UserId;
        if (userId == null)
            throw new UnauthorizedException("Bạn cần đăng nhập để cập nhật profile");

        // Lấy user từ database
        var user = await _userRepository.GetByIdAsync(userId.Value);
        if (user == null)
            throw new NotFoundException("Không tìm thấy user");

        // Cập nhật các field nếu có giá trị
        if (!string.IsNullOrWhiteSpace(request.FullName))
            user.FullName = request.FullName;
            
        if (request.Gender != null)
            user.Gender = request.Gender;
            
        if (request.Phone != null)
            user.Phone = request.Phone;
            
        if (request.Address != null)
            user.Address = request.Address;
            
        if (request.Bio != null)
            user.Bio = request.Bio;
            
        if (request.AvatarUrl != null)
            user.AvatarUrl = request.AvatarUrl;

        // Lưu vào database
        user = await _userRepository.UpdateAsync(user);

        return user.Adapt<UserProfileDto>();
    }
}
