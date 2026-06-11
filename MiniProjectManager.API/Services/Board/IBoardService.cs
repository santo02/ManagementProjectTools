using MiniProjectManager.API.DTOs;
using MiniProjectManager.API.Entities;

namespace MiniProjectManager.API.Services
{
    public interface IBoardService
    {
        Task<IEnumerable<Board>> GetAllBoard();
        Task<BoardResponseDto> CreateBoard(string Title, int WorkspaceId);
        Task<IEnumerable<BoardResponseDto>> GetBoardsByWorkspace(int workspaceId);
    }
}