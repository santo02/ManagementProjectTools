using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniProjectManager.API.DTOs;
using MiniProjectManager.API.Services;
using SQLitePCL;

namespace MiniProjectManager.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class TaskItemController : ControllerBase
    {
        public readonly ITaskItemService _taskItemService;

        public TaskItemController(ITaskItemService taskItemService)
        {
            _taskItemService = taskItemService;
        }

        [HttpGet("board/{boardId}")]
        public async Task<IActionResult> GetByBoardId(int boardId)
        {
            var task = await _taskItemService.GetTasksByBoardIdAsync(boardId);

            var response = task.Select(t => new TaskItemResponseDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                CreatedAt = t.CreatedAt,
                BoardId = t.BoardId,
                BoardTitle = t.Board?.Title ?? string.Empty,
                Priority = t.Priority,
                AssigneeId = t.AssigneeId,
                AssigneeName = t.Assignee?.Username
            });

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTaskItemDto req)
        {
            var newTaskItem = await _taskItemService.CreateTaskAsync(req
            );

            return Created(string.Empty, new { Message = "Tugas Berhasil dibuat!", TaskId = newTaskItem.Id });
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTaskItemDto req)
        {
            var updateTask = await _taskItemService.UpdateTaskAsync(id, req);

            if (updateTask == null)
            {
                return NotFound(new { Message = "Task Tidak Ditemukan!" });
            }

            return Ok(new { Message = "Task Berhasil Diupdate!", TaskId = updateTask.Id });
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var isDeleted = await _taskItemService.DeleteTaskAsync(id);

            if (!isDeleted)
            {
                return NotFound(new { Message = "Task Tidak Ditemukan" });
            }

            return Ok(new { Message = "Task Berhasil dihapus!" });
        }
    }
}