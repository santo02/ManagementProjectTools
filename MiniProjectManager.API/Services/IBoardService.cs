using MiniProjectManager.API.Entities;

namespace MiniProjectManager.API.Services
{
    public interface IBoardService
    {
        Task<IEnumerable<Board>> GetAllBoard();
        Task<Board> CreateBoard(string Title, int WorkspaceId, int Position);
    }
}