using Microsoft.EntityFrameworkCore;
using MonkeyArtInventory.Core.Models;

namespace MonkeyArtInventory.Data;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(InventoryDbContext dbContext)
    {
        try
        {
            await dbContext.Database.MigrateAsync();
        }
        catch
        {
            // If migrations cannot be applied for any reason, fall back to EnsureCreated
            await dbContext.Database.EnsureCreatedAsync();
        }

        // Ensure database exists (fallback if migrations didn't run)
        await dbContext.Database.EnsureCreatedAsync();

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
