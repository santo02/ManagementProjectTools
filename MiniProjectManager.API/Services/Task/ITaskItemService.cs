using MiniProjectManager.API.DTOs;
using MiniProjectManager.API.Entities;

namespace MiniProjectManager.API.Services
{
    public interface ITaskItemService
    {
        Task<IEnumerable<TaskItemResponseDto>> GetTasksByBoardIdAsync(int boardId);

        Task<TaskItemResponseDto?> CreateTaskAsync(CreateTaskItemDto req);

        Task<TaskItem?> UpdateTaskAsync(int Id, UpdateTaskItemDto req);
        Task<bool> DeleteTaskAsync(int Id);

        Task<bool> UpdateTaskBoardAsync(int taskId, int newBoardId);
    }
}