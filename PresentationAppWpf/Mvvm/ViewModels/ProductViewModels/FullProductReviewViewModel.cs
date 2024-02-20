using ApplicationCore.ProductCatalog.Dtos;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace PresentationAppWpf.Mvvm.ViewModels.ProductViewModels;

public partial class FullProductReviewViewModel(IServiceProvider serviceProvider) : BaseViewModel
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    [ObservableProperty]
    private ProductReviewDto? productReviewDto;

    [RelayCommand]
    private async Task NavigateToProductReviewList()
    {
        var productReviewViewModel = _serviceProvider.GetRequiredService<ProductReviewViewModel>();
        await productReviewViewModel.LoadProductReviewsAsync();
        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = productReviewViewModel;  
    }
}
