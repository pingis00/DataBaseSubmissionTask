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

public partial class ProductInventoryViewModel(IServiceProvider serviceProvider, IInventoryService inventoryService) : ObservableObject
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly IInventoryService _inventoryService = inventoryService;

    [ObservableProperty]
    public ObservableCollection<InventoryDto> inventories = [];
    private InventoryDto? _selectedInventory;

    public InventoryDto SelectedInventory
    {
        get => _selectedInventory!;
        set => SetProperty(ref _selectedInventory, value);
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

    public async Task LoadInventoryAsync()
    {
        var result = await _inventoryService.GetAllInventoriesAsync();

        if (result != null && result.IsSuccess)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Inventories = new ObservableCollection<InventoryDto>(result.Data);
                OnPropertyChanged(nameof(Inventories));
            });
        }
        else
        {
            ShowMessage("Produktlistanlistan är tom.");
        }
    }

    [RelayCommand]
    private async Task NavigateToUpdateInventory(int inventoryId)
    {
        var updateDtoResult = await _inventoryService.GetInventoriesByIdAsync(inventoryId);
        if (!updateDtoResult.IsSuccess || updateDtoResult.Data == null)
        {
            ShowMessage("Kunde inte hämta inventariet.");
            return;
        }

        var updateViewModel = _serviceProvider.GetRequiredService<UpdateInventoryViewModel>();
        await updateViewModel.InitializeInventory();
        updateViewModel.InventoryDto = updateDtoResult.Data;

        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = updateViewModel;
    }

    [RelayCommand]
    private void NavigateBack()
    {
        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = _serviceProvider.GetRequiredService<HomePageViewModel>();
    }
}
