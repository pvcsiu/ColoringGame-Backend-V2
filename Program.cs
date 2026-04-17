using Microsoft.EntityFrameworkCore;
using ColoringGame.API.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Kết nối DB
builder.Services.AddDbContext<ColoringGameDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Fix lỗi CORS (Cho phép Python gọi API)
builder.Services.AddCors(options => {
    options.AddDefaultPolicy(policy => {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors();

// CẤU HÌNH CHO PHÉP TẢI FILE TĨNH (ẢNH & .NPZ)
app.UseStaticFiles(new StaticFileOptions
{
    ServeUnknownFileTypes = true,
    DefaultContentType = "application/octet-stream"
});

app.MapControllers();

app.Run();