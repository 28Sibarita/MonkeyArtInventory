using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MonkeyArtInventory.Core.Services;

namespace MonkeyArtInventory.Data;

public class InventoryDbContextFactory : IDesignTimeDbContextFactory<InventoryDbContext>
{
    public InventoryDbContext CreateDbContext(string[] args)
    {
        var settingsService = new SettingsService();
        settingsService.Load();

        var optionsBuilder = new DbContextOptionsBuilder<InventoryDbContext>();
        var dbPath = settingsService.Current.DatabasePath;
        optionsBuilder.UseSqlite($"Data Source={dbPath}");

        return new InventoryDbContext(optionsBuilder.Options);
    }
}
