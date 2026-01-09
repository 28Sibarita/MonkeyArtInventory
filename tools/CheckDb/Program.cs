using Microsoft.Data.Sqlite;

var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MonkeyArtInventory", "Data", "inventory.db");
Console.WriteLine($"DB Path: {dbPath}");

if (!File.Exists(dbPath))
{
    Console.WriteLine("DB file does not exist.");
    return 0;
}

try
{
    using var conn = new SqliteConnection($"Data Source={dbPath}");
    conn.Open();

    using var cmd = conn.CreateCommand();
    cmd.CommandText = "SELECT name, type, sql FROM sqlite_master WHERE type IN ('table','index') ORDER BY type, name;";

    using var reader = cmd.ExecuteReader();
    while (reader.Read())
    {
        var name = reader.GetString(0);
        var type = reader.GetString(1);
        var sql = reader.IsDBNull(2) ? "" : reader.GetString(2);
        Console.WriteLine($"{type}: {name}\n  {sql}");
    }
        Console.WriteLine();
        Console.WriteLine("-- __EFMigrationsHistory entries --");
        using var cmd2 = conn.CreateCommand();
        cmd2.CommandText = "SELECT MigrationId, ProductVersion FROM __EFMigrationsHistory ORDER BY MigrationId;";
        using var reader2 = cmd2.ExecuteReader();
        while (reader2.Read())
        {
            Console.WriteLine($"{reader2.GetString(0)}  ({reader2.GetString(1)})");
        }
}
catch (Exception ex)
{
    Console.WriteLine("Exception while opening DB: " + ex);
}

return 0;