using ApplicationCore.Business.Dtos;
using ApplicationCore.Business.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using PresentationAppWpf.Validation;
using System.Collections.ObjectModel;


namespace PresentationAppWpf.Mvvm.ViewModels;

public partial class UpdateCustomerViewModel(IServiceProvider serviceProvider, ICustomerService customerService, IRoleService roleService, IContactPreferenceService contactPreferenceService) : BaseViewModel
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ICustomerService _customerService = customerService;
    private readonly IRoleService _roleService = roleService;
    private readonly IContactPreferenceService _contactPreferenceService = contactPreferenceService;
    private FormValidator? _formValidator;

    [ObservableProperty]
    private UpdateCustomerDto? updateCustomerDto;
    public ObservableCollection<RoleDto> AvailableRoles { get; private set; } = [];
    public ObservableCollection<ContactPreferenceDto> AvailableContactMethods { get; private set; } = [];

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

    [RelayCommand]
    private async Task NavigateBack()
    {
        var customerListViewModel = _serviceProvider.GetRequiredService<CustomerListViewModel>();
        await customerListViewModel.LoadCustomersAsync();
        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = customerListViewModel;
    }
}
