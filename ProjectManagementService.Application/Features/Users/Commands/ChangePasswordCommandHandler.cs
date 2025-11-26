using MediatR;
using Microsoft.AspNetCore.Identity;
using ProjectManagementService.Application.Interfaces;
using ProjectManagementService.Domain.Entities;
using ProjectManagementService.Domain.Exceptions;

namespace ProjectManagementService.Application.Features.Users.Commands;

/// <summary>
/// Handler đổi password
/// </summary>
public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, bool>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPasswordHasher<User> _passwordHasher;

    public ChangePasswordCommandHandler(
        IUserRepository userRepository,
        ICurrentUserService currentUserService,
        IPasswordHasher<User> passwordHasher)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
        _passwordHasher = passwordHasher;
    }

    public async Task<bool> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        // Lấy UserId từ JWT token
        var userId = _currentUserService.UserId;
        if (userId == null)
            throw new UnauthorizedException("Bạn cần đăng nhập để đổi mật khẩu");

        // Lấy user từ database
        var user = await _userRepository.GetByIdAsync(userId.Value);
        if (user == null)
            throw new NotFoundException("Không tìm thấy user");

        // Verify current password
        var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.CurrentPassword);
        if (verifyResult == PasswordVerificationResult.Failed)
            throw new ValidationException("Mật khẩu hiện tại không đúng");

        // Check new password và confirm password
        if (request.NewPassword != request.ConfirmPassword)
            throw new ValidationException("Mật khẩu mới và xác nhận mật khẩu không khớp");

        // Hash new password
        user.PasswordHash = _passwordHasher.HashPassword(user, request.NewPassword);

        // Lưu vào database
        await _userRepository.UpdateAsync(user);

        return true;
    }
}
