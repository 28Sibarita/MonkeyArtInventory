using Microsoft.EntityFrameworkCore;
using MonkeyArtInventory.Core.Models;

namespace MonkeyArtInventory.Data;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(InventoryDbContext dbContext)
    {
        // Try to apply migrations first
        bool migrationApplied = false;
        try
        {
            await dbContext.Database.MigrateAsync();
            migrationApplied = true;
        }
        catch
        {
            // If migrations fail, do nothing here - we'll try EnsureCreated below
        }

        // If migrations didn't work, ensure database is created
        if (!migrationApplied)
        {
            await dbContext.Database.EnsureCreatedAsync();
        }

        // Seed initial data if needed
        if (!await dbContext.Products.AnyAsync())
        {
            var seed = new List<Product>
            {
                new() { Name = "Tio Rico", Type = ProductType.Muneco3D, Barcode = "MONK-TR", StockMinimo = 0, IsActive = true },
                new() { Name = "Tasmania", Type = ProductType.Muneco3D, Barcode = "MONK-TAS", StockMinimo = 0, IsActive = true },
                new() { Name = "Boss Boni", Type = ProductType.Muneco3D, Barcode = "MONK-BB", StockMinimo = 0, IsActive = true },
                new() { Name = "Hombre Arana", Type = ProductType.Muneco3D, Barcode = "MONK-HA", StockMinimo = 0, IsActive = true }
            };

            dbContext.Products.AddRange(seed);
            await dbContext.SaveChangesAsync();
        }
    }
}
