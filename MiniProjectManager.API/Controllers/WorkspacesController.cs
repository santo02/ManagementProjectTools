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
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();
            int currentUserId = int.Parse(userIdClaim);

            var workspace = await _workspaceService.GetAllWorkspace(currentUserId);

            var response = workspace.Select(w => new WorkspaceResponseDto
            {
                Id = w.Id,
                Name = w.Name,
                CreatedAt = w.CreatedAt.Year == 1 ? DateTime.UtcNow : w.CreatedAt,

                CreatorName = w.User != null && !string.IsNullOrWhiteSpace(w.User.Username)
                              ? w.User.Username
                              : "Leader"
            });

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateWorkspaceDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Nama project tidak terbaca oleh server atau kosong.");

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();
            int currentUserId = int.Parse(userIdClaim);

            // Kirim currentUserId ke service agar LeaderId dan UserId terisi dengan benar
            var result = await _workspaceService.CreateWorkspace(dto.Name, currentUserId);
            return Ok(result);
        }

        [HttpPost("{workspaceId}/members")]
        public async Task<IActionResult> AddMember(int workspaceId, [FromBody] AddMemberRequestDto request)
        {
            try
            {
                var userIdClaim = GetUserId();
                if (string.IsNullOrEmpty(userIdClaim.ToString())) return Unauthorized("User ID not found in token.");

                await _workspaceService.Addmember(workspaceId, request, userIdClaim);

                return Ok(new { Message = "Member added successfully." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

            [HttpGet("my-workspaces")]
            public async Task<IActionResult> GetUserWorkspaces()
            {
                try
                {
                    // Coba ambil menggunakan standar NameIdentifier
                    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (string.IsNullOrEmpty(userIdClaim))
                    {
                        userIdClaim = User.FindFirst("id")?.Value ?? User.FindFirst("nameid")?.Value;
                    }

                    if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized("Token tidak mengandung klaim ID User.");

                    int currentUserId = int.Parse(userIdClaim);

                    var workspaces = await _workspaceService.GetUserWorkspacesAsync(currentUserId);
                    return Ok(workspaces);
                }
                catch (Exception ex)
                {
                    // Ubah sementara ke ex.Message agar Anda bisa melihat jika ada error parsing/konversi di terminal log
                    return StatusCode(500, $"Terjadi kesalahan internal: {ex.Message}");
                }
            }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateWorkspaceDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name)) return BadRequest("Nama project tidak boleh kosong.");

            int currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "1");
            var result = await _workspaceService.UpdateWorkspaceAsync(id, dto.Name, currentUserId);

            if (result == null) return Forbid("Anda tidak memiliki akses untuk mengubah project ini.");
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            int currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "1");
            var success = await _workspaceService.DeleteWorkspaceAsync(id, currentUserId);

            if (!success) return Forbid("Gagal menghapus. Project tidak ditemukan atau Anda bukan Leader.");
            return Ok(new { message = "Project berhasil dihapus secara permanen!" });
        }
    }
}