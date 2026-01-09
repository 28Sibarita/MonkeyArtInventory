using Microsoft.EntityFrameworkCore;
using MonkeyArtInventory.Core.Interfaces;
using MonkeyArtInventory.Core.Models;

namespace MonkeyArtInventory.Data.Services;

public class InventoryRepository : IInventoryRepository
{
    private readonly InventoryDbContext _db;

    public InventoryRepository(InventoryDbContext db)
    {
        _db = db;
    }

    // Products
    public Task<List<Product>> ListProductsAsync()
    {
        return _db.Products.AsNoTracking().ToListAsync();
    }

    public Task<Product?> FindProductByIdAsync(int id)
    {
        return _db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
    }

    public Task<Product?> FindProductByBarcodeAsync(string barcode)
    {
        return _db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Barcode == barcode);
    }

    public Task<bool> BarcodeExistsAsync(string barcode, int? excludingId = null)
    {
        return _db.Products.AnyAsync(p => p.Barcode == barcode && (!excludingId.HasValue || p.Id != excludingId.Value));
    }

    public Task AddProductAsync(Product product)
    {
        _db.Products.Add(product);
        return Task.CompletedTask;
    }

    public Task UpdateProductAsync(Product product)
    {
        _db.Products.Update(product);
        return Task.CompletedTask;
    }

    public Task DeleteProductAsync(Product product)
    {
        _db.Products.Remove(product);
        return Task.CompletedTask;
    }

    // Movements
    public Task<List<Movement>> ListMovementsAsync(DateTime? from = null, DateTime? to = null)
    {
        IQueryable<Movement> query = _db.Movements.AsNoTracking().Include(m => m.Client);

        if (from.HasValue)
        {
            query = query.Where(m => m.OccurredAt >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(m => m.OccurredAt < to.Value);
        }

        return query.ToListAsync();
    }

    public Task AddMovementAsync(Movement movement)
    {
        _db.Movements.Add(movement);
        return Task.CompletedTask;
    }

    // Clients
    public Task<List<Client>> ListClientsAsync()
    {
        return _db.Clients.AsNoTracking().ToListAsync();
    }

    public Task<Client?> FindClientByIdAsync(int id)
    {
        return _db.Clients.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
    }

    public Task<Client?> FindClientByNameAsync(string name)
    {
        return _db.Clients.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());
    }

    public Task AddClientAsync(Client client)
    {
        _db.Clients.Add(client);
        return Task.CompletedTask;
    }

    public Task UpdateClientAsync(Client client)
    {
        _db.Clients.Update(client);
        return Task.CompletedTask;
    }

    public Task DeleteClientAsync(Client client)
    {
        _db.Clients.Remove(client);
        return Task.CompletedTask;
    }

    public Task<List<Movement>> GetClientMovementsAsync(int clientId)
    {
        return _db.Movements
            .AsNoTracking()
            .Include(m => m.Product)
            .Where(m => m.ClientId == clientId)
            .OrderByDescending(m => m.OccurredAt)
            .ToListAsync();
    }

    public Task SaveChangesAsync()
    {
        return _db.SaveChangesAsync();
    }
}
