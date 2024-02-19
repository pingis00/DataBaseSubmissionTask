using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using PresentationAppWpf.Mvvm.ViewModels.ProductViewModels;

namespace PresentationAppWpf.Mvvm.ViewModels;

public partial class HomePageViewModel(IServiceProvider serviceProvider) : ObservableObject
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    [RelayCommand]
    private async Task NavigateToSettings()
    {
        var settingsViewModel = _serviceProvider.GetRequiredService<SettingsViewModel>();
        await settingsViewModel.InitializeAsync();

        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = settingsViewModel;
    }

    [RelayCommand]
    private async Task NavigateToRegisterCustomer()
    {
        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        var registerViewModel = _serviceProvider.GetRequiredService<RegisterCustomerViewModel>();

        await registerViewModel.InitializeAsync();
        mainViewModel.CurrentViewModel = registerViewModel;
    }
    [RelayCommand]
    private async Task NavigateToCustomerList()
    {
        var customerListViewModel = _serviceProvider.GetRequiredService<CustomerListViewModel>();
        await customerListViewModel.LoadCustomersAsync();

        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = customerListViewModel;
    }
    [RelayCommand]
    private async Task NavigateToCustomerReview()
    {
        var customerReviewViewModel = _serviceProvider.GetRequiredService<CustomerReviewViewModel>();
        await customerReviewViewModel.LoadReviewsAsync();

        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = customerReviewViewModel;
    }

    [RelayCommand]
    private async Task NavigateToProductInventory()
    {
        var settingsViewModel = _serviceProvider.GetRequiredService<ProductSettingsViewModel>();
        await settingsViewModel.InitializeCategoryAndBrandAsync();

        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = settingsViewModel;
    }

    [RelayCommand]
    private async Task NavigateToCreateProduct()
    {
        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        var createProductViewModel = _serviceProvider.GetRequiredService<CreateProductViewModel>();

        await createProductViewModel.LoadBrandsAndCategoriesAsync();
        mainViewModel.CurrentViewModel = createProductViewModel;
    }

    [RelayCommand]
    private async Task NavigateToProductList()
    {
        var productListViewModel = _serviceProvider.GetRequiredService<ProductListViewModel>();
        await productListViewModel.LoadProductsAsync();

        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = productListViewModel;
    }

    [RelayCommand]
    private async Task NavigateToProductReview()
    {
        var productReviewViewModel = _serviceProvider.GetRequiredService<ProductReviewViewModel>();
        await productReviewViewModel.LoadProductReviewsAsync();

        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = productReviewViewModel;
    }
}
