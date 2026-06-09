using Microsoft.EntityFrameworkCore;
using MiniProjectManager.API.Entities;

namespace MiniProjectManager.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Workspace> Workspaces { get; set; }
        public DbSet<Board> Boards { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<TaskItem> TaskItems { get; set; }
    }
}