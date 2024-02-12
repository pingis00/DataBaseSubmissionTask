﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

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
    private void NavigateToCustomerReview()
    {
        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = _serviceProvider.GetRequiredService<CustomerReviewViewModel>();
    }
}
