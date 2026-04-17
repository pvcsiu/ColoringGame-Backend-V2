using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders; // <-- Thêm thư viện quản lý file vật lý
using ColoringGame.API.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Kết nối DB
builder.Services.AddDbContext<ColoringGameDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Fix lỗi CORS
builder.Services.AddCors(options => {
    options.AddDefaultPolicy(policy => {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors();

// --- BẢN VÁ LỖI TẢI FILE ---
// 1. Ép tạo thư mục wwwroot ngay khi Server vừa bật
var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
if (!Directory.Exists(webRootPath)) 
{
    Directory.CreateDirectory(webRootPath);
}

// 2. Ép Server phải phục vụ các file nằm trong thư mục này
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(webRootPath),
    RequestPath = "",
    ServeUnknownFileTypes = true,
    DefaultContentType = "application/octet-stream"
});
// ---------------------------

app.MapControllers();

app.Run();