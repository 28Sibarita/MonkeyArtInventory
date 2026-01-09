using MonkeyArtInventory.Core.Models;

namespace MonkeyArtInventory.Core.Interfaces;

public interface IInventoryRepository
{
    // Products
    Task<List<Product>> ListProductsAsync();
    Task<Product?> FindProductByIdAsync(int id);
    Task<Product?> FindProductByBarcodeAsync(string barcode);
    Task<bool> BarcodeExistsAsync(string barcode, int? excludingId = null);
    Task AddProductAsync(Product product);
    Task UpdateProductAsync(Product product);
    Task DeleteProductAsync(Product product);

    // Movements
    Task<List<Movement>> ListMovementsAsync(DateTime? from = null, DateTime? to = null);
    Task AddMovementAsync(Movement movement);

    // Clients
    Task<List<Client>> ListClientsAsync();
    Task<Client?> FindClientByIdAsync(int id);
    Task<Client?> FindClientByNameAsync(string name);
    Task AddClientAsync(Client client);
    Task UpdateClientAsync(Client client);
    Task DeleteClientAsync(Client client);
    Task<List<Movement>> GetClientMovementsAsync(int clientId);

    Task SaveChangesAsync();
}
