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

            string finalCreatorName = "Leader";
            if (user != null && !string.IsNullOrWhiteSpace(user.Username))
            {
                finalCreatorName = user.Username;
            }

            return new WorkspaceResponseDto
            {
                Id = newWorkspace.Id,
                Name = newWorkspace.Name,
                LeaderId = newWorkspace.LeaderId,
                // PENGAMANAN TANGGAL: Jika EF Core mereset nilainya menjadi tahun 0001 setelah SaveChanges, paksa gunakan waktu saat ini
                CreatedAt = newWorkspace.CreatedAt.Year == 1 ? DateTime.UtcNow : newWorkspace.CreatedAt,
                CreatorName = finalCreatorName
            };
        }

        public async Task<IEnumerable<Workspace>> GetAllWorkspace(int userId)
        {
            return await _context.Workspaces
            .Include(w => w.User)
            .Where(w => w.LeaderId == userId)
            .ToListAsync();
        }

        public async Task<IEnumerable<WorkspaceResponseDto>> GetUserWorkspacesAsync(int userId)
        {
            return await _context.Workspaces
                // EF Core akan otomatis melakukan JOIN ke tabel User saat kita memanggil w.User di dalam Select
                .Where(w => w.LeaderId == userId || _context.WorkspaceMembers.Any(wm => wm.WorkspaceId == w.Id && wm.UserId == userId))
                .Select(w => new WorkspaceResponseDto
                {
                    Id = w.Id,
                    Name = w.Name,
                    LeaderId = w.LeaderId,
                    CreatedAt = w.CreatedAt,
                    CreatorName = w.User != null ? w.User.Username : "Unknown"
                })
                .ToListAsync();
        }

        public async Task<bool> IsLeaderAsync(int workspaceId, int userId)
        {
            return await _context.Workspaces.AnyAsync(w => w.Id == workspaceId && w.LeaderId == userId);
        }

        public async Task<WorkspaceResponseDto?> UpdateWorkspaceAsync(int id, string name, int userId)
        {
            var workspace = await _context.Workspaces.FindAsync(id);

            // Pengaman: Jika tidak ditemukan atau user bukan pemilik/leader proyek
            if (workspace == null || workspace.LeaderId != userId) return null;

            workspace.Name = name;
            await _context.SaveChangesAsync();

            var user = await _context.Users.FindAsync(userId);

            return new WorkspaceResponseDto
            {
                Id = workspace.Id,
                Name = workspace.Name,
                LeaderId = workspace.LeaderId,
                CreatedAt = workspace.CreatedAt,
                CreatorName = user?.Username ?? "Unknown"
            };
        }

        public async Task<bool> DeleteWorkspaceAsync(int id, int userId)
        {
            var workspace = await _context.Workspaces.FindAsync(id);

            // Pengaman: Hanya Leader yang boleh menghapus
            if (workspace == null || workspace.LeaderId != userId) return false;

            // Efek Domino: Hapus juga data board dan member yang berelasi dengan workspace ini
            var relatedBoards = _context.Boards.Where(b => b.WorkspaceId == id);
            var relatedMembers = _context.WorkspaceMembers.Where(m => m.WorkspaceId == id);

            _context.Boards.RemoveRange(relatedBoards);
            _context.WorkspaceMembers.RemoveRange(relatedMembers);
            _context.Workspaces.Remove(workspace);

            await _context.SaveChangesAsync();
            return true;
        }
    }
}