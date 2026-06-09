using MiniProjectManager.API.DTOs;
using MiniProjectManager.API.Entities;

namespace MiniProjectManager.API.Services
{
    public interface ITaskItemService
    {
        Task<IEnumerable<TaskItem>> GetTasksByBoardIdAsync(int boardId);

        Task<TaskItem> CreateTaskAsync(CreateTaskItemDto req);

        Task<TaskItem?> UpdateTaskAsync(int Id, UpdateTaskItemDto req);
        Task<bool> DeleteTaskAsync(int Id);
    }
}