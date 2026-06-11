using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniProjectManager.API.DTOs;
using MiniProjectManager.API.Services;

namespace MiniProjectManager.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[Controller]")]

    public class BoardController : ControllerBase
    {
        private readonly IBoardService _boardService;
        public BoardController(IBoardService boardService)
        {
            _boardService = boardService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var board = await _boardService.GetAllBoard();

            var response = board.Select(b => new BoardResponseDto
            {
                Id = b.Id,
                Title = b.Title,
                WorkspaceId = b.WorkspaceId
            });

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BoardResponseDto req)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "1");

            try
            {
                var newBoard = await _boardService.CreateBoard(req.Title, req.WorkspaceId);
                return Ok(newBoard);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("workspace/{workspaceId}")]
        public async Task<IActionResult> GetBoardsByWorkspace(int workspaceId)
        {
            var boards = await _boardService.GetBoardsByWorkspace(workspaceId);

            return Ok(boards);
        }
    }
}