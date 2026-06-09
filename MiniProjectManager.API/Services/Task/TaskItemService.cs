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

        async Task<TaskItem?> ITaskItemService.CreateTaskAsync(CreateTaskItemDto req)
        {
            var newTaskItem = new TaskItem
            {
                Title = req.Title,
                Description = req.Description,
                BoardId = req.BoardId,
                AssigneeId = req.AssigneeId,
                Priority = req.Priority,
                CreatedAt = DateTime.UtcNow
            };

            _context.TaskItems.Add(newTaskItem);

            await _context.SaveChangesAsync();

            return newTaskItem;
        }

        async Task<IEnumerable<TaskItem>> ITaskItemService.GetTasksByBoardIdAsync(int boardId)
        {
            return await _context.TaskItems
                                .Include(t => t.Board)
                                .Include(t => t.Assignee)
                                .Where(t => t.BoardId == boardId)
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

    }
}