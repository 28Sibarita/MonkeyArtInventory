using System.Windows;
using System.Windows.Controls;
using MonkeyArtInventory.App.ViewModels;

namespace MonkeyArtInventory.App.Views;

public partial class SettingsView
{
    public SettingsView()
    {
        InitializeComponent();
        Loaded += SettingsView_Loaded;
    }

    private void SettingsView_Loaded(object sender, RoutedEventArgs e)
    {
        // Load password from ViewModel when view loads
        if (DataContext is SettingsViewModel vm)
        {
            PasswordBox.Password = vm.SmtpPassword;
        }
    }

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        // Update ViewModel when password changes
        if (DataContext is SettingsViewModel vm)
        {
            vm.SmtpPassword = PasswordBox.Password;
        }
    }
}
