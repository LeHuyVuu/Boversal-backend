using ProjectManagementService.Domain.Entities;

namespace ProjectManagementService.Application.Interfaces;

// Interface cho JWT token service
public interface IJwtService
{
    string GenerateToken(User user);
    long? ValidateToken(string token);
}
