using ApplicationCore.Business.Dtos;
using ApplicationCore.Business.Helpers;
using ApplicationCore.Business.Interfaces;
using ApplicationCore.Business.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using PresentationAppWpf.Validation;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace PresentationAppWpf.Mvvm.ViewModels;

public partial class UpdateCustomerViewModel : ObservableObject
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ICustomerService _customerService;
    private readonly IRoleService _roleService;
    private readonly IContactPreferenceService _contactPreferenceService;
    private FormValidator? _formValidator;

    [ObservableProperty]
    private UpdateCustomerDto? updateCustomerDto;
    public ObservableCollection<RoleDto> AvailableRoles { get; private set; } = [];
    public ObservableCollection<ContactPreferenceDto> AvailableContactMethods { get; private set; } = [];

    public UpdateCustomerViewModel(IServiceProvider serviceProvider, ICustomerService customerService, IRoleService roleService, IContactPreferenceService contactPreferenceService)
    {
        _serviceProvider = serviceProvider;
        _customerService = customerService;
        _roleService = roleService;
        _contactPreferenceService = contactPreferenceService;
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

    public async Task InitializeAsync()
    {
        await LoadRolesAndPreferencesAsync();
    }

    public async Task LoadRolesAndPreferencesAsync()
    {
        var rolesResult = await _roleService.GetAllRolesAsync();
        if (rolesResult.IsSuccess)
        {
            AvailableRoles.Clear();
            foreach (var role in rolesResult.Data)
            {
                AvailableRoles.Add(role);
            }
        }
        else
        {
            ShowMessage("Något gick fel");
        }

        var preferencesResult = await _contactPreferenceService.GetAllContactPreferencesAsync();
        if (preferencesResult.IsSuccess)
        {
            AvailableContactMethods.Clear();
            foreach (var preference in preferencesResult.Data)
            {
                AvailableContactMethods.Add(preference);
            }
        }
        else
        {
            ShowMessage("Något gick fel");
        }
    }

    private void InitializeFormValidator()
    {
        if (UpdateCustomerDto == null)
        {
            ShowMessage("Vänligen fyll i alla obligatoriska fält.");
        }
        else
        {
            _formValidator = new FormValidator(UpdateCustomerDto, ShowMessage);
        }
    }

    [RelayCommand]
    private async Task UpdateCustomer()
    {
        if (UpdateCustomerDto == null)
        {
            ShowMessage("Uppdateringsdata är inte korrekt inställd.");
            return;
        }

        InitializeFormValidator();

        if (_formValidator == null)
        {
            return;
        }
        if (!_formValidator.ValidateForm())
        {
            return;
        }

        var result = await _customerService.UpdateCustomerAsync(UpdateCustomerDto);
        if (result.IsSuccess)
        {
            ShowMessage("Kunduppdatering lyckades.");
            await NavigateBackToListView();
        }
        else
        {
            ShowMessage("Kunduppdatering misslyckades.");
        }
    }
    private async Task NavigateBackToListView()
    {
        var customerListViewModel = _serviceProvider.GetRequiredService<CustomerListViewModel>();
        await customerListViewModel.LoadCustomersAsync();
        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = customerListViewModel;
    }
}
