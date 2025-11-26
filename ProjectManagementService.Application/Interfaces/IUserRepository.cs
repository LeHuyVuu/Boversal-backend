using ProjectManagementService.Domain.Entities;

namespace ProjectManagementService.Application.Interfaces;

// Interface cho User repository
public interface IUserRepository
{
    Task<User?> GetByIdAsync(long id);
    Task<User?> GetByEmailAsync(string email);
    Task<User> AddAsync(User user);
    Task<User> UpdateAsync(User user);
    Task<bool> EmailExistsAsync(string email);
    Task<List<User>> SearchUsersAsync(string? searchTerm, int limit);
}
