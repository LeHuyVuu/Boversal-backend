using Mapster;
using MediatR;
using ProjectManagementService.Application.Interfaces;
using ProjectManagementService.Application.DTOs.Auth;

namespace ProjectManagementService.Application.Features.Auth.Commands;

/// <summary>
/// Handler xử lý đăng nhập
/// 1. Map LoginCommand -> LoginDto bằng Mapster
/// 2. Gọi AuthService.LoginAsync() để validate và tạo JWT token
/// 3. Trả về AuthResponseDto chứa token và thông tin user
/// </summary>
public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly IAuthService _authService;

    public LoginCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // Dùng Mapster map LoginCommand -> LoginDto
        var loginDto = request.Adapt<LoginDto>();

        // Gọi service xử lý login
        return await _authService.LoginAsync(loginDto);
    }
}
