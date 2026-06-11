using Microsoft.EntityFrameworkCore;
using MiniProjectManager.API.Data;
using MiniProjectManager.API.DTOs;
using MiniProjectManager.API.Entities;

namespace MiniProjectManager.API.Services
{
    public class TaskItemService : ITaskItemService
    {
        private readonly AppDbContext _context;

        public TaskItemService(AppDbContext context)
        {
            _context = context;
        }

        async Task<TaskItemResponseDto?> ITaskItemService.CreateTaskAsync(CreateTaskItemDto dto)
        {
            var task = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                Priority = dto.Priority,
                // Progress = 0, 
                // DueDate = dto.DueDate,
                BoardId = dto.BoardId,
                CreatedAt = DateTime.UtcNow
            };

            _context.TaskItems.Add(task);
            await _context.SaveChangesAsync();


            return new TaskItemResponseDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Priority = task.Priority,
                // Progress = task.Progress,
                // DueDate = task.DueDate,
                BoardId = task.BoardId
            };
        }

        async Task<IEnumerable<TaskItemResponseDto>> ITaskItemService.GetTasksByBoardIdAsync(int boardId)
        {
            return await _context.TaskItems
                 .Where(t => t.BoardId == boardId)
                 .Select(t => new TaskItemResponseDto
                 {
                     Id = t.Id,
                     Title = t.Title,
                     Description = t.Description,
                     Priority = t.Priority,
                     //  Progress = t.Progress,
                     //  DueDate = t.DueDate,
                     BoardId = t.BoardId
                 })
                 .ToListAsync();

        }
        public async Task<bool> DeleteTaskAsync(int Id)
        {
            var task = await _context.TaskItems.FindAsync(Id);
            if (task == null)
            {
                return false;
            }

            _context.TaskItems.Remove(task);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<TaskItem?> UpdateTaskAsync(int Id, UpdateTaskItemDto req)
        {
            var task = await _context.TaskItems.FindAsync(Id);
            if (task == null)
            {
                return null;
            }

            task.Title = req.Title;
            task.Description = req.Description;
            task.BoardId = req.BoardId;
            task.AssigneeId = req.AssigneeId;

            await _context.SaveChangesAsync();

            return task;
        }

        public async Task<bool> UpdateTaskBoardAsync(int taskId, int newBoardId)
        {
            var task = await _context.TaskItems.FindAsync(taskId);
            if (task == null) return false;

            task.BoardId = newBoardId;
            await _context.SaveChangesAsync();
            return true;
        }

    }
}