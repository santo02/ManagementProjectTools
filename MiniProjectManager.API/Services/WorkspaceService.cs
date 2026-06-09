using Microsoft.EntityFrameworkCore;
using MiniProjectManager.API.Data;
using MiniProjectManager.API.Entities;

namespace MiniProjectManager.API.Services
{
    public class WorkspaceService : IWorkspaceService
    {
        private readonly AppDbContext _context;

        public WorkspaceService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Workspace> CreateWorkspace(string name, int userId)
        {
            var newWorkspace = new Workspace
            {
                Name = name,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };
            
            _context.Workspaces.Add(newWorkspace);

            await _context.SaveChangesAsync();

            return newWorkspace;
        }

        public async Task<IEnumerable<Workspace>> GetAllWorkspace(int userId)
        {
            return await _context.Workspaces
            .Include(w => w.User)
            .Where(w => w.UserId == userId)
            .ToListAsync();
        }
    }
}