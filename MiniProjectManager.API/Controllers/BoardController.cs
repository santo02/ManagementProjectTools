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
            var newBoard = await _boardService.CreateBoard(req.Title, req.WorkspaceId, req.Position);

            var response = new BoardResponseDto
            {
                Id = newBoard.Id,
                Title = newBoard.Title,
                Position = newBoard.Position,
                WorkspaceId = newBoard.WorkspaceId
            };

            return Created(String.Empty, response);
        }
    }
}