using PresentationAppWpf.Mvvm.ViewModels;

namespace PresentationAppWpf.Helpers;

public class NavigationService
{
    private readonly MainViewModel _mainViewModel;

    public NavigationService(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
    }
}
