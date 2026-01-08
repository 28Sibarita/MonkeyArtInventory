using System.Windows.Input;
using System.Windows;

namespace MonkeyArtInventory.App.Views;

public partial class InventoryView
{
    public InventoryView()
    {
        InitializeComponent();
    }

    private void SearchBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            var code = SearchBox.Text?.Trim();
            if (!string.IsNullOrWhiteSpace(code) && DataContext is MonkeyArtInventory.App.ViewModels.InventoryViewModel vm)
            {
                vm.HandleScannedBarcode(code);
            }

            SearchBox.Clear();
            e.Handled = true;
        }
    }
}
