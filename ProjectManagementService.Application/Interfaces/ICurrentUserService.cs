namespace ProjectManagementService.Application.Interfaces;

/// <summary>
/// Current user service interface - get current authenticated user info
/// Implement in Infrastructure layer using HttpContext or JWT claims
/// </summary>
public interface ICurrentUserService
{
    long? UserId { get; }
    string? UserName { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
    List<string> Roles { get; }
}
