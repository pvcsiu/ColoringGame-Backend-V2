using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ColoringGame.API.Models;

[Table("userprogress")] // 👈 THÊM DÒNG NÀY ĐỂ ÉP CỨNG TÊN BẢNG KHÔNG CÓ CHỮ "ES"
public class UserProgress
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ProgressId { get; set; }
    public int UserId { get; set; }
    public int ArtworkId { get; set; }
    public int ColoredRegionsCount { get; set; } = 0;
    public string Status { get; set; } = "Playing";
    public string? ColoredStatus { get; set; }
    public string? ColoringHistory { get; set; }
    public DateTime? LastSyncedAt { get; set; }
}