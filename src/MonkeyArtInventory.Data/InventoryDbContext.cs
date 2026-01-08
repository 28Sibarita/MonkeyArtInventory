using Microsoft.EntityFrameworkCore;
using MonkeyArtInventory.Core.Models;

namespace MonkeyArtInventory.Data;

public class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Movement> Movements => Set<Movement>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
            entity.Property(p => p.Barcode).IsRequired().HasMaxLength(100);
            entity.HasIndex(p => p.Barcode).IsUnique();
            entity.Property(p => p.Location).HasMaxLength(100);
            entity.Property(p => p.ImagePath).HasMaxLength(260);
            entity.Property(p => p.SalePrice).HasColumnType("decimal(18,2)");
            entity.Property(p => p.StockMinimo).HasDefaultValue(0);
            entity.Property(p => p.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<Movement>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Note).HasMaxLength(500);
            entity.Property(m => m.UnitPrice).HasColumnType("decimal(18,2)");
            entity.HasOne(m => m.Product)
                .WithMany(p => p.Movements)
                .HasForeignKey(m => m.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
            // Client-related mapping
            entity.Property(m => m.ClientName).HasMaxLength(200);
            entity.Property(m => m.ClientPrice).HasColumnType("decimal(18,2)");
            entity.Property(m => m.IsConsignment).HasDefaultValue(false);
            entity.Property(m => m.ClientNote).HasMaxLength(500);
        });
    }
}
