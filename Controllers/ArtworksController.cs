using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ColoringGame.API.Models;

namespace ColoringGame.API.Controllers;

[Route("api/artworks")]
[ApiController]
public class ArtworksController : ControllerBase
{
    private readonly ColoringGameDbContext _context;

    public ArtworksController(ColoringGameDbContext context)
    {
        _context = context;
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetGallery(int userId)
    {
        var gallery = await _context.Artworks
            .Select(a => new {
                ArtworkId = a.ArtworkId, Title = a.Title, TotalColors = a.TotalColors,
                TotalRegions = a.TotalRegions, DataUrl = a.DataUrl, ThumbnailUrl = a.ThumbnailUrl,
                Progress = _context.UserProgresses.FirstOrDefault(p => p.ArtworkId == a.ArtworkId && p.UserId == userId)
            }).ToListAsync();

        var result = gallery.Select(g => new {
            g.ArtworkId, g.Title, g.TotalColors, g.TotalRegions, g.DataUrl, g.ThumbnailUrl,
            ColoredRegions = g.Progress != null ? g.Progress.ColoredRegionsCount : 0,
            Status = g.Progress != null ? g.Progress.Status : "New"
        });

        return Ok(result);
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadArtwork([FromForm] string title, [FromForm] int totalColors, [FromForm] int totalRegions, IFormFile dataFile, IFormFile thumbFile)
    {
        if (dataFile == null || thumbFile == null) return BadRequest(new { message = "Thiếu file!" });

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

        var dataFileName = Guid.NewGuid().ToString() + "_data.npz";
        var thumbFileName = Guid.NewGuid().ToString() + "_thumb.png";

        using (var stream = new FileStream(Path.Combine(uploadsFolder, dataFileName), FileMode.Create)) await dataFile.CopyToAsync(stream);
        using (var stream = new FileStream(Path.Combine(uploadsFolder, thumbFileName), FileMode.Create)) await thumbFile.CopyToAsync(stream);

        // Tự động lấy URL của Render
        var serverUrl = $"{Request.Scheme}://{Request.Host}";

        var artwork = new Artwork {
            Title = title, TotalColors = totalColors, TotalRegions = totalRegions,
            DataUrl = $"{serverUrl}/uploads/{dataFileName}",
            ThumbnailUrl = $"{serverUrl}/uploads/{thumbFileName}",
            CreatedAt = DateTime.UtcNow
        };

        _context.Artworks.Add(artwork);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Upload thành công!" });
    }

    [HttpPost("update-thumb")]
    public async Task<IActionResult> UpdateThumbnail([FromForm] int artworkId, IFormFile thumbFile)
    {
        var artwork = await _context.Artworks.FindAsync(artworkId);
        if (artwork == null || thumbFile == null) return NotFound();

        var thumbFileName = Path.GetFileName(new Uri(artwork.ThumbnailUrl).LocalPath);
        var thumbPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", thumbFileName);

        using (var stream = new FileStream(thumbPath, FileMode.Create)) await thumbFile.CopyToAsync(stream);
        return Ok(new { message = "Đã cập nhật Thumbnail!" });
    }

    [HttpPost("delete")]
    public async Task<IActionResult> DeleteArtworks([FromBody] List<int> artworkIds)
    {
        var artworks = await _context.Artworks.Where(a => artworkIds.Contains(a.ArtworkId)).ToListAsync();
        var progresses = await _context.UserProgresses.Where(p => artworkIds.Contains(p.ArtworkId)).ToListAsync();
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

        foreach (var art in artworks)
        {
            var dataPath = Path.Combine(uploadsFolder, Path.GetFileName(new Uri(art.DataUrl).LocalPath));
            var thumbPath = Path.Combine(uploadsFolder, Path.GetFileName(new Uri(art.ThumbnailUrl).LocalPath));
            if (System.IO.File.Exists(dataPath)) System.IO.File.Delete(dataPath);
            if (System.IO.File.Exists(thumbPath)) System.IO.File.Delete(thumbPath);
        }

        _context.UserProgresses.RemoveRange(progresses);
        _context.Artworks.RemoveRange(artworks);
        await _context.SaveChangesAsync();

        return Ok(new { message = $"Đã xóa {artworks.Count} bức tranh!" });
    }
}