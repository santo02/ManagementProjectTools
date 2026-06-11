using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MiniProjectManager.API.Data;
using MiniProjectManager.API.Services;
// Scalar ditambahkan di sini
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Mengabaikan properti navigasi sirkular agar tidak menyebabkan crash
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<IWorkspaceService, WorkspaceService>();
builder.Services.AddScoped<IBoardService, BoardService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITaskItemService, TaskItemService>();

// MENGGUNAKAN NATIVE OPENAPI .NET 10 (Pengganti AddSwaggerGen)
builder.Services.AddOpenApi();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Validasi kunci rahasia (Ini yang PALING PENTING untuk keamanan)
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),

            // Validasi waktu kedaluwarsa token
            ValidateLifetime = true,

            // Matikan validasi string Issuer dan Audience untuk mode Development
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

// Wajib ditambahkan agar [Authorize] bisa bekerja
builder.Services.AddAuthorization();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // Mengizinkan akses khusus dari Vue Anda
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // Meng-generate file openapi.json (Pengganti UseSwagger)
    app.MapOpenApi();

    // Mengaktifkan UI interaktif Scalar (Pengganti UseSwaggerUI)
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Mini Project Manager API");
        options.WithTheme(ScalarTheme.Mars);
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();