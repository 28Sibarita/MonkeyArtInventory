using MonkeyArtInventory.Core.DTOs;
using MonkeyArtInventory.Core.Interfaces;
using MonkeyArtInventory.Core.Models;
using MonkeyArtInventory.Core.Utilities;

namespace MonkeyArtInventory.Core.Services;

public class ProductService
{
    private readonly IInventoryRepository _repository;

    public ProductService(IInventoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<ProductListItemDto>> GetListAsync(ProductListFilter filter)
    {
        var products = await _repository.ListProductsAsync();
        var movements = await _repository.ListMovementsAsync();

        var stockByProduct = movements
            .GroupBy(m => m.ProductId)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(m => IsIncrease(m.Type) ? m.Quantity : -m.Quantity));

        IEnumerable<Product> query = products;

        if (!string.IsNullOrWhiteSpace(filter.Query))
        {
            var term = filter.Query.Trim().ToLowerInvariant();
            query = query.Where(p => p.Name.ToLowerInvariant().Contains(term)
                || p.Barcode.ToLowerInvariant().Contains(term));
        }

        if (filter.Type.HasValue)
        {
            query = query.Where(p => p.Type == filter.Type.Value);
        }

        var list = query
            .Select(p =>
            {
                var stock = stockByProduct.TryGetValue(p.Id, out var value) ? value : 0;
                return new ProductListItemDto(
                    p.Id,
                    p.Name,
                    p.Barcode,
                    EnumHelper.GetDisplayName(p.Type),
                    stock,
                    p.StockMinimo,
                    p.Location,
                    p.SalePrice,
                    p.ImagePath,
                    p.IsActive);
            })
            .ToList();

        if (filter.OnlyLowStock)
        {
            list = list.Where(p => p.Stock <= p.StockMinimo).ToList();
        }

        if (filter.OnlyOutOfStock)
        {
            list = list.Where(p => p.Stock <= 0).ToList();
        }

        return list;
    }

    public Task<Product?> GetByIdAsync(int id) => _repository.FindProductByIdAsync(id);

    public Task<Product?> GetByBarcodeAsync(string barcode) => _repository.FindProductByBarcodeAsync(barcode);

    public async Task<ServiceResult> CreateAsync(Product product)
    {
        if (string.IsNullOrWhiteSpace(product.Name))
        {
            return ServiceResult.Fail("Name is required.");
        }

        if (string.IsNullOrWhiteSpace(product.Barcode))
        {
            return ServiceResult.Fail("Barcode is required.");
        }

        if (await _repository.BarcodeExistsAsync(product.Barcode, null))
        {
            return ServiceResult.Fail("Barcode already exists.");
        }

        await _repository.AddProductAsync(product);
        await _repository.SaveChangesAsync();
        return ServiceResult.Ok("Product created.");
    }

    public async Task<ServiceResult> UpdateAsync(Product product)
    {
        if (string.IsNullOrWhiteSpace(product.Name))
        {
            return ServiceResult.Fail("Name is required.");
        }

        if (string.IsNullOrWhiteSpace(product.Barcode))
        {
            return ServiceResult.Fail("Barcode is required.");
        }

        if (await _repository.BarcodeExistsAsync(product.Barcode, product.Id))
        {
            return ServiceResult.Fail("Barcode already exists.");
        }

        await _repository.UpdateProductAsync(product);
        await _repository.SaveChangesAsync();
        return ServiceResult.Ok("Product updated.");
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var product = await _repository.FindProductByIdAsync(id);
        if (product is null)
        {
            return ServiceResult.Fail("Product not found.");
        }

        await _repository.DeleteProductAsync(product);
        await _repository.SaveChangesAsync();
        return ServiceResult.Ok("Product deleted.");
    }

    public async Task<int> GetStockAsync(int productId)
    {
        var movements = await _repository.ListMovementsAsync();
        return movements
            .Where(m => m.ProductId == productId)
            .Sum(m => IsIncrease(m.Type) ? m.Quantity : -m.Quantity);
    }

    private static bool IsIncrease(MovementType type)
    {
        return type == MovementType.Entrada || type == MovementType.Ajuste;
    }
}
