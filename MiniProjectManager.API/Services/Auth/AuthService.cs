using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MiniProjectManager.API.Data;
using MiniProjectManager.API.DTOs;
using MiniProjectManager.API.Entities;

namespace MiniProjectManager.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto request)
        {
            // Cari user di database
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);

            var token = GenerateJwtToken(user);

            // PERBARUI RETURN NYA:
            return new AuthResponseDto
            {
                Token = token,
                User = new UserPayloadDto
                {
                    Id = user.Id,
                    Username = user.Username
                }
            };
        }

        public async Task<string> RegisterAsync(RegisterDto req)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Username == req.Username);
            if (userExists)
            {
                throw new Exception("Username sudah terdaftar");
            }

            string PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password);

            var newUser = new User
            {
                Username = req.Username,
                Password = PasswordHash
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return "Registrasi Berhasil!";
        }

        private string GenerateJwtToken(User u)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, u.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, u.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audiance"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}