using ApplicationCore.Business.Dtos;
using ApplicationCore.Business.Interfaces;
using ApplicationCore.Business.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace PresentationAppWpf.Mvvm.ViewModels;

public partial class CustomerListViewModel(IServiceProvider serviceProvider, ICustomerService customerService) : ObservableObject
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ICustomerService _customerService = customerService;

    [ObservableProperty]
    public ObservableCollection<CustomerListDto> customers = [];
    private CustomerListDto? _selectedCustomer;
    public CustomerListDto SelectedCustomer
    {
        get => _selectedCustomer!;
        set => SetProperty(ref _selectedCustomer, value);
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

    public async Task LoadCustomersAsync()
    {
        var result = await _customerService.GetAllCustomersAsync();

        if (result != null && result.IsSuccess)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Customers = new ObservableCollection<CustomerListDto>(result.Data);
                OnPropertyChanged(nameof(Customers));
            });
        }
        else
        {
            ShowMessage("Inventarielistan är tom.");
        }
    }

    public async Task DeleteCustomerAsync(int customerId)
    {
        var result = await _customerService.DeleteCustomerAsync(customerId);

        Application.Current.Dispatcher.Invoke(() =>
        {
            if (result.IsSuccess)
            {
                var customerToRemove = Customers.First(c => c.Id == customerId);
                Customers.Remove(customerToRemove);
                ShowMessage("Kunden raderades");
            }
            else
            {
                ShowMessage("Det gick inte att radera kunden");
            }
        });
    }

    [RelayCommand]
    private async Task DeleteCustomer(int customerId)
    {
        await DeleteCustomerAsync(customerId);
    }

    [RelayCommand]
    private async Task NavigateToUpdateCustomer(int customerId)
    {
        var updateDtoResult = await _customerService.GetCustomerByIdAsync(customerId);
        if (!updateDtoResult.IsSuccess || updateDtoResult.Data == null)
        {
            ShowMessage("Kunde inte hämta kundinformation.");
            return;
        }

        var updateViewModel = _serviceProvider.GetRequiredService<UpdateCustomerViewModel>();
        await updateViewModel.InitializeAsync();
        updateViewModel.UpdateCustomerDto = updateDtoResult.Data;

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
