using ApplicationCore.ProductCatalog.Dtos;
using ApplicationCore.ProductCatalog.Interfaces;
using ApplicationCore.ProductCatalog.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Windows;

namespace PresentationAppWpf.Mvvm.ViewModels.ProductViewModels;

public partial class ProductListViewModel(IServiceProvider serviceProvider, IProductService productService) : ObservableObject
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly IProductService _productService = productService;

    [ObservableProperty]
    public ObservableCollection<CompleteProductDto> products = [];
    private CompleteProductDto? _selectedProduct;

    public CompleteProductDto SelectedProduct
    {
        get => _selectedProduct!;
        set => SetProperty(ref _selectedProduct, value);
    }

    private SnackbarMessageQueue _messageQueue = new(TimeSpan.FromSeconds(3));

    public SnackbarMessageQueue MessageQueue
    {
        get { return _messageQueue; }
        set { SetProperty(ref _messageQueue, value); }
    }

    public void ShowMessage(string message)
    {
        MessageQueue.Enqueue(message);
    }

    public async Task LoadProductsAsync()
    {
        var result = await _productService.GetAllProductsAsync();

        if (result != null && result.IsSuccess)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Products = new ObservableCollection<CompleteProductDto>(result.Data);
                OnPropertyChanged(nameof(Products));
            });
        }
        else
        {
            ShowMessage("Produktlistanlistan är tom.");
        }
    }

    public async Task DeleteProductAsync(int productId)
    {
        var result = await _productService.DeleteProductAsync(productId);

        Application.Current.Dispatcher.Invoke(() =>
        {
            if (result.IsSuccess)
            {
                var productsToRemove = Products.First(p => p.Id == productId);
                Products.Remove(productsToRemove);
                ShowMessage("Produkten raderades");
            }
            else
            {
                ShowMessage("Det gick inte att radera produkten");
            }
        });
    }

    [RelayCommand]
    private async Task DeleteProduct(int productId)
    {
        await DeleteProductAsync(productId);
    }

    [RelayCommand]
    private async Task NavigateToUpdateProducts(int productId)
    {
        var updateDtoResult = await _productService.GetProductByIdAsync(productId);
        if (!updateDtoResult.IsSuccess || updateDtoResult.Data == null)
        {
            ShowMessage("Kunde inte hämta produktinformation.");
            return;
        }

        var updateViewModel = _serviceProvider.GetRequiredService<UpdateProductViewModel>();
        await updateViewModel.InitializeBrandsAndCategories();
        updateViewModel.UpdateProductDto = _productService.ConvertToUpdatable(updateDtoResult.Data);

        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = updateViewModel;
    }

    [RelayCommand]
    private async Task NavigateToProductDetails(int productId)
    {
        var productDtoResult = await _productService.GetProductByIdAsync(productId);
        if (!productDtoResult.IsSuccess || productDtoResult.Data == null)
        {
            ShowMessage("Kunde inte hämta Produktinformation.");
            return;
        }

        var fullProductViewModel = _serviceProvider.GetRequiredService<FullProductViewModel>();
        fullProductViewModel.CompleteProductDto = productDtoResult.Data;

        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = fullProductViewModel;
    }

    [RelayCommand]
    private void NavigateBack()
    {
        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = _serviceProvider.GetRequiredService<HomePageViewModel>();
    }
}
