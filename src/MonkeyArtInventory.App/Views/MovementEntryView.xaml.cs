using System.Windows;
using System.Windows.Input;

namespace MonkeyArtInventory.App.Views;

public partial class MovementEntryView
{
    public MovementEntryView()
    {
        InitializeComponent();
        Loaded += MovementEntryView_Loaded;
    }

    private void MovementEntryView_Loaded(object sender, RoutedEventArgs e)
    {
        // Ensure the barcode textbox has keyboard focus so the scanner (HID) can type directly
        BarcodeTextBox.Focus();
        Keyboard.Focus(BarcodeTextBox);
    }
}
