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

    public Task<List<Product>> ListProductsAsync()
    {
        return _db.Products.AsNoTracking().ToListAsync();
    }

    public Task<List<Movement>> ListMovementsAsync(DateTime? from = null, DateTime? to = null)
    {
        IQueryable<Movement> query = _db.Movements.AsNoTracking();

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

    public Task AddMovementAsync(Movement movement)
    {
        _db.Movements.Add(movement);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync()
    {
        return _db.SaveChangesAsync();
    }
}
