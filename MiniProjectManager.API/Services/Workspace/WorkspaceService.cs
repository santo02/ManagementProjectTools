using Microsoft.EntityFrameworkCore;
using MiniProjectManager.API.Data;
using MiniProjectManager.API.DTOs;
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

        public async Task<bool> Addmember(int workspaceId, AddMemberRequestDto request, int CurrentUserId)
        {
            var isLeader = await IsLeaderAsync(workspaceId, CurrentUserId);
            if (!isLeader)
            {
                throw new UnauthorizedAccessException("Only workspace leaders can add members.");
            }

            var targetUser = _context.Users.FirstOrDefault(u => u.Username == request.Username);
            if (targetUser == null)
            {
                throw new ArgumentException("User not found.");
            }

            var isAlreadyMember = await _context.WorkspaceMembers
                .AnyAsync(wm => wm.WorkspaceId == workspaceId && wm.UserId == targetUser.Id);

            if (isAlreadyMember)
            {
                throw new InvalidOperationException("User sudah menjadi anggota di workspace ini.");
            }

            var newMember = new WorkspaceMember
            {
                WorkspaceId = workspaceId,
                UserId = targetUser.Id,
                Role = "Member",
                JoinedAt = DateTime.UtcNow
            };

            _context.WorkspaceMembers.Add(newMember);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<WorkspaceResponseDto> CreateWorkspace(string name, int userId)
        {
            // 1. Simpan Workspace Baru
            var newWorkspace = new Workspace
            {
                Name = name,
                UserId = userId,
                LeaderId = userId,
                CreatedAt = DateTime.UtcNow
            };
            _context.Workspaces.Add(newWorkspace);
            await _context.SaveChangesAsync();

            // 2. Simpan Anggota Pertama (Leader)
            var firstMember = new WorkspaceMember
            {
                WorkspaceId = newWorkspace.Id,
                UserId = userId,
                Role = "Leader",
                JoinedAt = DateTime.UtcNow
            };
            _context.WorkspaceMembers.Add(firstMember);

            // ===============================================================
            // OLEH-OLEH OTOMATIS: BUAT 3 KOLOM BAWAAN UNTUK KANBAN BARU
            // ===============================================================
            var defaultBoards = new List<Board>
            {
                new Board { Title = "To Do", Position = 1, WorkspaceId = newWorkspace.Id },
                new Board { Title = "In Progress", Position = 2, WorkspaceId = newWorkspace.Id },
                new Board { Title = "Done", Position = 3, WorkspaceId = newWorkspace.Id }
            };
            _context.Boards.AddRange(defaultBoards);

            await _context.SaveChangesAsync();

            var user = await _context.Users.FindAsync(userId);

            return new WorkspaceResponseDto
            {
                Id = newWorkspace.Id,
                Name = newWorkspace.Name,
                LeaderId = newWorkspace.LeaderId,
                CreatedAt = newWorkspace.CreatedAt,
                CreatorName = user?.Username ?? "Unknown"
            };
        }

        public async Task<IEnumerable<Workspace>> GetAllWorkspace(int userId)
        {
            return await _context.Workspaces
            .Include(w => w.User)
            .Where(w => w.UserId == userId)
            .ToListAsync();
        }

        public async Task<IEnumerable<WorkspaceResponseDto>> GetUserWorkspacesAsync(int userId)
        {
            return await _context.Workspaces
        .Where(w => w.LeaderId == userId || _context.WorkspaceMembers.Any(wm => wm.WorkspaceId == w.Id && wm.UserId == userId))
        .Select(w => new WorkspaceResponseDto
        {
            Id = w.Id,
            Name = w.Name,
            LeaderId = w.LeaderId
        })
        .ToListAsync();
        }

        public async Task<bool> IsLeaderAsync(int workspaceId, int userId)
        {
            return await _context.Workspaces.AnyAsync(w => w.Id == workspaceId && w.LeaderId == userId);
        }
    }
}