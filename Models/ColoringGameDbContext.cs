using Microsoft.EntityFrameworkCore;

namespace ColoringGame.API.Models;

public class ColoringGameDbContext : DbContext
{
    public ColoringGameDbContext(DbContextOptions<ColoringGameDbContext> options) : base(options) { }

    public DbSet<Artwork> Artworks { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserProgress> UserProgresses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Ràng buộc Unique: 1 User chỉ có 1 tiến độ trên 1 bức tranh
        modelBuilder.Entity<UserProgress>()
            .HasIndex(p => new { p.UserId, p.ArtworkId })
            .IsUnique();

        // PHÉP THUẬT ÉP CHỮ THƯỜNG CHO POSTGRESQL
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            var tableName = entity.GetTableName();
            if (tableName != null) 
                entity.SetTableName(tableName.ToLower());

            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(property.Name.ToLower());
            }
        }
    }
}