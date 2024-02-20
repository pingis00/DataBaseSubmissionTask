using ApplicationCore.ProductCatalog.Dtos;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace PresentationAppWpf.Mvvm.ViewModels.ProductViewModels;

public partial class FullProductViewModel(IServiceProvider serviceProvider) : BaseViewModel
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    [ObservableProperty]
    private CompleteProductDto? completeProductDto;

    [RelayCommand]
    private async Task NavigateToProductList()
    {
        var productListViewModel = _serviceProvider.GetRequiredService<ProductListViewModel>();
        await productListViewModel.LoadProductsAsync();
        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = productListViewModel;
    }
}
