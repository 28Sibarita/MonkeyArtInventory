using MonkeyArtInventory.Core.Models;

namespace MonkeyArtInventory.Core.Interfaces;

public interface IInventoryRepository
{
    Task<List<Product>> ListProductsAsync();
    Task<List<Movement>> ListMovementsAsync(DateTime? from = null, DateTime? to = null);
    Task<Product?> FindProductByIdAsync(int id);
    Task<Product?> FindProductByBarcodeAsync(string barcode);
    Task<bool> BarcodeExistsAsync(string barcode, int? excludingId = null);
    Task AddProductAsync(Product product);
    Task UpdateProductAsync(Product product);
    Task DeleteProductAsync(Product product);
    Task AddMovementAsync(Movement movement);
    Task SaveChangesAsync();
}
