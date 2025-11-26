using Mapster;
using MediatR;
using ProjectManagementService.Application.Interfaces;
using ProjectManagementService.Application.DTOs.Auth;

namespace ProjectManagementService.Application.Features.Auth.Commands;

/// <summary>
/// Handler xử lý đăng ký tài khoản mới
/// 1. Map RegisterCommand -> RegisterDto bằng Mapster
/// 2. Gọi AuthService.RegisterAsync() để tạo user và JWT token
/// 3. Trả về AuthResponseDto chứa token và thông tin user
/// </summary>
public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    private readonly IAuthService _authService;

    public RegisterCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // Dùng Mapster map RegisterCommand -> RegisterDto
        var registerDto = request.Adapt<RegisterDto>();

        // Gọi service xử lý register
        return await _authService.RegisterAsync(registerDto);
    }
}
