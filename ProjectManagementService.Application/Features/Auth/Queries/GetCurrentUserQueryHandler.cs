using MediatR;
using ProjectManagementService.Application.Interfaces;
using ProjectManagementService.Application.DTOs.Auth;
using ProjectManagementService.Domain.Exceptions;

namespace ProjectManagementService.Application.Features.Auth.Queries;

/// <summary>
/// Handler lấy thông tin user hiện tại
/// CƠ CHẾ HOẠT ĐỘNG:
/// 1. Client gửi request kèm JWT token trong header: Authorization: Bearer {token}
/// 2. ASP.NET Core Authentication Middleware tự động validate token
/// 3. Middleware đọc Claims từ token và gán vào HttpContext.User
/// 4. CurrentUserService đọc UserId từ HttpContext.User.Claims
/// 5. AuthService.GetCurrentUserAsync() query database lấy thông tin user
/// 6. Trả về UserDto
/// </summary>
public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, UserDto>
{
    private readonly IAuthService _authService;

    public GetCurrentUserQueryHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<UserDto> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        // Lấy thông tin user từ token (CurrentUserService tự động đọc từ HttpContext)
        var user = await _authService.GetCurrentUserAsync();
        
        if (user == null)
            throw new NotFoundException("Không tìm thấy thông tin user hoặc token không hợp lệ");

        return user;
    }
}
