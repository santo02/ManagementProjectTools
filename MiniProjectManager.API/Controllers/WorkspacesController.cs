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
    public class WorkspaceController : ControllerBase
    {
        private readonly IWorkspaceService _workspaceService;

        public WorkspaceController(IWorkspaceService workspaceService)
        {
            _workspaceService = workspaceService;
        }

        private int GetUserId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(userIdString!);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = GetUserId();
            var workspace = await _workspaceService.GetAllWorkspace(userId);

            var response = workspace.Select(w => new WorkspaceResponseDto
            {
                Id = w.Id,
                Name = w.Name,
                CreatedAt = w.CreatedAt,
                CreatorName = w.User!.Username
            });

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] WorkspaceResponseDto request)
        {
            int UserId = GetUserId();
            var newWorkspace = await _workspaceService.CreateWorkspace(request.Name, UserId);

            var response = new WorkspaceResponseDto
            {
                Id = newWorkspace.Id,
                Name = newWorkspace.Name,
                CreatedAt = newWorkspace.CreatedAt
            };

            return Created(String.Empty, response);
        }
    }
}