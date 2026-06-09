using Microsoft.AspNetCore.Mvc;
using MiniProjectManager.API.DTOs;
using MiniProjectManager.API.Services;

namespace MiniProjectManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto request)
        {
            try
            {
                var result = await _authService.RegisterAsync(request);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                // Jika username sudah ada, tangkap errornya dan kembalikan 400 Bad Request
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            var result = await _authService.LoginAsync(request);

            if (result == null)
            {
                // Kembalikan 401 Unauthorized jika login gagal
                return Unauthorized(new { message = "Username atau password salah." });
            }

            return Ok(result);
        }
    }
}