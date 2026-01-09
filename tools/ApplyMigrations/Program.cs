using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MonkeyArtInventory.Data;
using MonkeyArtInventory.Core.Services;

var services = new ServiceCollection();
var settingsService = new MonkeyArtInventory.Core.Services.SettingsService();
settingsService.Load();

services.AddSingleton(settingsService);
services.AddDbContext<InventoryDbContext>(options =>
{
    options.UseSqlite($"Data Source={settingsService.Current.DatabasePath}");
});

var sp = services.BuildServiceProvider();
using var scope = sp.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();

Console.WriteLine($"Applying migrations to DB: {settingsService.Current.DatabasePath}");
try
{
    await db.Database.MigrateAsync();
    Console.WriteLine("MigrateAsync completed.");
}
catch (Exception ex)
{
    Console.WriteLine("MigrateAsync failed: " + ex);
}

Console.WriteLine("Schema after migration:");
using var conn = db.Database.GetDbConnection();
conn.Open();
using var cmd = conn.CreateCommand();
cmd.CommandText = "SELECT name, type FROM sqlite_master WHERE type IN ('table','index') ORDER BY type, name;";
using var reader = cmd.ExecuteReader();
while (reader.Read())
{
    Console.WriteLine($"{reader.GetString(1)}: {reader.GetString(0)}");
}

// If Products table still missing, create tables directly (fallback)
using var checkCmd = conn.CreateCommand();
checkCmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='Products';";
var exists = checkCmd.ExecuteScalar();
if (exists is null)
{
    Console.WriteLine("Products table missing — creating tables directly as fallback...");

    using var createCmd = conn.CreateCommand();
    createCmd.CommandText = @"CREATE TABLE IF NOT EXISTS Products (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Type INTEGER NOT NULL,
    Barcode TEXT NOT NULL,
    StockMinimo INTEGER NOT NULL DEFAULT 0,
    Location TEXT,
    SalePrice decimal(18,2),
    ImagePath TEXT,
    IsActive INTEGER NOT NULL DEFAULT 1
);

CREATE TABLE IF NOT EXISTS Movements (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Type INTEGER NOT NULL,
    OccurredAt TEXT NOT NULL,
    ProductId INTEGER NOT NULL,
    Quantity INTEGER NOT NULL,
    Note TEXT,
    UnitPrice decimal(18,2),
    FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE
);

CREATE UNIQUE INDEX IF NOT EXISTS IX_Products_Barcode ON Products(Barcode);
CREATE INDEX IF NOT EXISTS IX_Movements_ProductId ON Movements(ProductId);";
    createCmd.ExecuteNonQuery();

    Console.WriteLine("Fallback tables created.");
}

return 0;