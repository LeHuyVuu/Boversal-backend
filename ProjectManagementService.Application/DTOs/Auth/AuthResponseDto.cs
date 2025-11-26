namespace ProjectManagementService.Application.DTOs.Auth;

// DTO cho response sau khi login/register
public class AuthResponseDto
{
    public long UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}
