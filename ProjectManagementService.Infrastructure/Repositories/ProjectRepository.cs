using Microsoft.EntityFrameworkCore;
using ProjectManagementService.Application.Interfaces;
using ProjectManagementService.Domain.Entities;
using ProjectManagementService.Infrastructure.Persistence;

namespace ProjectManagementService.Infrastructure.Repositories;

public class ProjectRepository : IProjectRepository
    {
        private readonly MyDbContext _db;

        public ProjectRepository(MyDbContext db)
        {
            _db = db;
        }

        public async Task<List<Project>> GetAllAsync()
        {
            return await _db.Projects.Include(project => project.ProjectStatuses).ToListAsync();
        }

        public async Task<List<Project>> GetPagedAsync(int pageNumber, int pageSize)
        {
            return await _db.Projects
                .OrderByDescending(p => p.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _db.Projects.CountAsync();
        }

        public async Task<Project?> GetByIdAsync(long id)
        {
            return await _db.Projects.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<long> CreateAsync(Project project)
        {
            _db.Projects.Add(project);
            await _db.SaveChangesAsync();
            return project.Id;
        }

        public async Task<bool> UpdateAsync(Project project)
        {
            _db.Projects.Update(project);
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var project = await GetByIdAsync(id);
            if (project == null)
                return false;

            _db.Projects.Remove(project);
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<List<Project>> GetUserProjectsAsync(long userId)
        {
            return await _db.Projects
                .Include(p => p.Tasks.Where(t => t.DeletedAt == null))
                .Where(p => p.OwnerId == userId && p.DeletedAt == null)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }
    }
