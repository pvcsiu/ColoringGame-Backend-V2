using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ColoringGame.API.Models;

public class Artwork
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ArtworkId { get; set; }
    public string Title { get; set; } = null!;
    public int TotalColors { get; set; }
    public int TotalRegions { get; set; }
    public string DataUrl { get; set; } = null!;
    public string ThumbnailUrl { get; set; } = null!;
    public DateTime? CreatedAt { get; set; }
}