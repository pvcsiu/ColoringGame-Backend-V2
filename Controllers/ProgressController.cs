using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ColoringGame.API.Models;

namespace ColoringGame.API.Controllers;

[Route("api/progress")]
[ApiController]
public class ProgressController : ControllerBase
{
    private readonly ColoringGameDbContext _context;

    public ProgressController(ColoringGameDbContext context)
    {
        _context = context;
    }

    [HttpGet("get")]
    public async Task<IActionResult> GetProgress(int userId, int artworkId)
    {
        var progress = await _context.UserProgresses.FirstOrDefaultAsync(p => p.UserId == userId && p.ArtworkId == artworkId);
        if (progress == null) return NotFound(new { message = "Chưa có tiến độ." });
        return Ok(progress);
    }

    [HttpPost("sync")]
    public async Task<IActionResult> SyncProgress([FromBody] SyncProgressRequest request)
    {
        // 1. Kiểm tra tồn tại để chống lỗi 500
        if (!await _context.Artworks.AnyAsync(a => a.ArtworkId == request.ArtworkId))
            return NotFound(new { message = "Lỗi: Không tìm thấy bức tranh này!" });

        var progress = await _context.UserProgresses.FirstOrDefaultAsync(p => p.UserId == request.UserId && p.ArtworkId == request.ArtworkId);

        if (progress == null)
        {
            progress = new UserProgress {
                UserId = request.UserId, ArtworkId = request.ArtworkId,
                ColoredRegionsCount = request.ColoredRegionsCount, ColoredStatus = request.ColoredStatus,
                ColoringHistory = request.ColoringHistory, LastSyncedAt = DateTime.UtcNow,
                Status = request.ColoredRegionsCount >= request.TotalRegions ? "Completed" : "Playing"
            };
            _context.UserProgresses.Add(progress);
        }
        else
        {
            progress.ColoredRegionsCount = request.ColoredRegionsCount;
            progress.ColoredStatus = request.ColoredStatus;
            progress.ColoringHistory = request.ColoringHistory;
            progress.LastSyncedAt = DateTime.UtcNow;
            progress.Status = request.ColoredRegionsCount >= request.TotalRegions ? "Completed" : "Playing";
        }

        await _context.SaveChangesAsync();
        return Ok(new { message = "Đã đồng bộ thành công!" });
    }
}

public class SyncProgressRequest
{
    public int UserId { get; set; }
    public int ArtworkId { get; set; }
    public int TotalRegions { get; set; }
    public int ColoredRegionsCount { get; set; }
    public string ColoredStatus { get; set; } = string.Empty;
    public string ColoringHistory { get; set; } = string.Empty;
}