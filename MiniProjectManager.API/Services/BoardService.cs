using Microsoft.EntityFrameworkCore;
using MiniProjectManager.API.Data;
using MiniProjectManager.API.Entities;

namespace MiniProjectManager.API.Services
{
    public class BoardService : IBoardService{
        private readonly AppDbContext _context;

        public BoardService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Board> CreateBoard(string title, int position, int workspaceId)
        {
           var newBoard = new Board
           {
             Title = title,
             WorkspaceId = workspaceId,
             Position = position
           };

           _context.Boards.Add(newBoard);

           await _context.SaveChangesAsync();

           return newBoard;
        }

        public async Task<IEnumerable<Board>> GetAllBoard()
        {
            return await _context.Boards.ToListAsync();
        }
    }
}