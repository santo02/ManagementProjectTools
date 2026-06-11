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
        public async Task<IActionResult> GetTasksByBoard(int boardId)
        {
            try
            {
                var tasks = await _taskItemService.GetTasksByBoardIdAsync(boardId);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Terjadi kesalahan internal: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTaskItemDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.Title)) return BadRequest("Judul tugas tidak boleh kosong.");

                var result = await _taskItemService.CreateTaskAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Gagal membuat tugas: {ex.Message}");
            }
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

        [HttpPut("{id}/move")]
        public async Task<IActionResult> MoveTask(int id, [FromBody] MoveTaskDto dto)
        {
            var success = await _taskItemService.UpdateTaskBoardAsync(id, dto.NewBoardId);
            if (!success) return NotFound("Tugas tidak ditemukan.");
            return Ok(new { message = "Tugas berhasil dipindahkan!" });
        }

        public class MoveTaskDto { public int NewBoardId { get; set; } }
    }
}