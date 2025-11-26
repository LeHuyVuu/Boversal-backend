using ProjectManagementService.Application.DTOs.Auth;

namespace ProjectManagementService.Application.Interfaces;

// Interface cho Authentication service
public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
    Task<UserDto?> GetCurrentUserAsync();
    string HashPassword(string password);
    bool VerifyPassword(string password, string hashedPassword);
}
