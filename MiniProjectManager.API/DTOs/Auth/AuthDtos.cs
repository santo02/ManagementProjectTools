namespace MiniProjectManager.API.DTOs
{
    public class RegisterDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
    public class LoginDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class UserPayloadDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
    }

    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;

        // TAMBAHKAN INI: Agar data user ikut terkirim saat login sukses
        public UserPayloadDto User { get; set; } = new UserPayloadDto();
    }
}