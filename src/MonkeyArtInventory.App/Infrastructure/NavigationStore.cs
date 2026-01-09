using CommunityToolkit.Mvvm.ComponentModel;

namespace MonkeyArtInventory.App.Infrastructure;

public class NavigationStore : ObservableObject
{
    private ObservableObject? _currentViewModel;

    public ObservableObject? CurrentViewModel
    {
        get => _currentViewModel;
        set => SetProperty(ref _currentViewModel, value);
    }
}
