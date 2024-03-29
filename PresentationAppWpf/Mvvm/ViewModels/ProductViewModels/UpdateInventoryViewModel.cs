﻿using ApplicationCore.ProductCatalog.Dtos;
using ApplicationCore.ProductCatalog.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;

namespace PresentationAppWpf.Mvvm.ViewModels.ProductViewModels;

public partial class UpdateInventoryViewModel(IServiceProvider serviceProvider, IInventoryService inventoryService, IProductService productService) : BaseViewModel
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly IInventoryService _inventoryService = inventoryService;
    private readonly IProductService _productService = productService;

    [ObservableProperty]
    private InventoryDto? inventoryDto;
    public ObservableCollection<CompleteProductDto> AvailableProducts { get; private set; } = [];

    public async Task InitializeInventory()
    {
        await LoadInventoriesAsync();
    }

    public async Task LoadInventoriesAsync()
    {
        var productResult = await _productService.GetAllProductsAsync();
        if (productResult.IsSuccess)
        {
            AvailableProducts.Clear();
            foreach (var product in productResult.Data)
            {
                AvailableProducts.Add(product);
            }
        }
        else
        {
            ShowMessage("Något gick fel");
        }
    }


    [RelayCommand]
    private async Task UpdateProductInventory()
    {
        if (InventoryDto == null)
        {
            ShowMessage("Uppdateringsdata är inte korrekt inställd.");
            return;
        }

        var result = await _inventoryService.UpdateInventoryAsync(InventoryDto);
        if (result.IsSuccess)
        {
            ShowMessage("Inventarieuppdatering lyckades.");
            await NavigateBackToProductView();
        }
        else
        {
            ShowMessage("Inventarieuppdatering misslyckades.");
        }
    }
    private async Task NavigateBackToProductView()
    {
        var productInventoryViewModel = _serviceProvider.GetRequiredService<ProductInventoryViewModel>();
        await productInventoryViewModel.LoadInventoryAsync();
        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = productInventoryViewModel;
    }

    [RelayCommand]
    private async Task NavigateBack()
    {
        await NavigateBackToProductView();
    }
}
