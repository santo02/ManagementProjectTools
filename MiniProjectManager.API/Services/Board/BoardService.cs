using Microsoft.EntityFrameworkCore;
using MiniProjectManager.API.Data;
using MiniProjectManager.API.DTOs;
using MiniProjectManager.API.Entities;

namespace MiniProjectManager.API.Services
{
    public class BoardService : IBoardService
    {
        private readonly AppDbContext _context;

        public BoardService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<BoardResponseDto> CreateBoard(string title, int workspaceId)
        {
            var existingBoardsCount = await _context.Boards
                .Where(b => b.WorkspaceId == workspaceId)
                .CountAsync();

            var newBoard = new Board
            {
                Title = title,
                WorkspaceId = workspaceId,
                Position = existingBoardsCount + 1
            };

            _context.Boards.Add(newBoard);
            await _context.SaveChangesAsync();

            return new BoardResponseDto
            {
                Id = newBoard.Id,
                Title = newBoard.Title,
                Position = newBoard.Position,
                WorkspaceId = newBoard.WorkspaceId
            };
        }

        public async Task<IEnumerable<Board>> GetAllBoard()
        {
            return await _context.Boards.ToListAsync();
        }

        public async Task<IEnumerable<BoardResponseDto>> GetBoardsByWorkspace(int workspaceId)
        {
            return await _context.Boards
                .Where(b => b.WorkspaceId == workspaceId)
                .OrderBy(b => b.Position)
                .Select(b => new BoardResponseDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Position = b.Position,
                    WorkspaceId = b.WorkspaceId
                })
                .ToListAsync();
        }
    }
}