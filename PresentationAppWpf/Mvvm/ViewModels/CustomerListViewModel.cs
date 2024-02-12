using ApplicationCore.Business.Dtos;
using ApplicationCore.Business.Interfaces;
using ApplicationCore.Business.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace PresentationAppWpf.Mvvm.ViewModels;

public partial class CustomerListViewModel : ObservableObject
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ICustomerService _customerService;

    public CustomerListViewModel(IServiceProvider serviceProvider, ICustomerService customerService)
    {
        _serviceProvider = serviceProvider;
        _customerService = customerService;
    }

    [ObservableProperty]
    public ObservableCollection<CustomerListDto> customers = [];
    private CustomerListDto? _selectedCustomer;
    public CustomerListDto SelectedCustomer
    {
        get => _selectedCustomer!;
        set => SetProperty(ref _selectedCustomer, value);
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
            MessageBox.Show("Kunde inte hämta kundinformation.", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
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
