using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using MonkeyArtInventory.Core.Services;

namespace MonkeyArtInventory.Mac.ViewModels;

public partial class ProductDetailViewModel : ViewModelBase
{
    private readonly ProductService _productService;

    [ObservableProperty]
    private int _productId;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _barcode = string.Empty;

    [ObservableProperty]
    private int _currentStock;

    [ObservableProperty]
    private decimal? _salePrice;

    public ProductDetailViewModel()
    {
        _productService = App.Services!.GetRequiredService<ProductService>();
    }
}
