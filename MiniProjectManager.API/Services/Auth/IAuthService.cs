using MiniProjectManager.API.DTOs;

namespace MiniProjectManager.API.Services
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterDto req);
        Task<AuthResponseDto> LoginAsync(LoginDto req);

    }
}