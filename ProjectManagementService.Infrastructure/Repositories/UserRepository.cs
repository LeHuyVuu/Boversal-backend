using Microsoft.EntityFrameworkCore;
using ProjectManagementService.Application.Interfaces;
using ProjectManagementService.Domain.Entities;
using ProjectManagementService.Infrastructure.Persistence;

namespace ProjectManagementService.Infrastructure.Repositories;

// Repository cho User
public class UserRepository : IUserRepository
{
    private readonly MyDbContext _context;

    public UserRepository(MyDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(long id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User> AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User> UpdateAsync(User user)
    {
        user.UpdatedAt = DateTime.UtcNow;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }

    public async Task<List<User>> SearchUsersAsync(string? searchTerm, int limit)
    {
        var query = _context.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var search = searchTerm.ToLower();
            query = query.Where(u => 
                u.FullName.ToLower().Contains(search) || 
                u.Email.ToLower().Contains(search) ||
                (u.Username != null && u.Username.ToLower().Contains(search))
            );
        }

        return await query
            .OrderBy(u => u.FullName)
            .Take(limit)
            .ToListAsync();
    }
}
