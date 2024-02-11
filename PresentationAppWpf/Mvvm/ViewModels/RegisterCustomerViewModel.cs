using ApplicationCore.Business.Dtos;
using ApplicationCore.Business.Interfaces;
using ApplicationCore.Business.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using PresentationAppWpf.Validation;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PresentationAppWpf.Mvvm.ViewModels;

public partial class RegisterCustomerViewModel : ObservableObject
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ICustomerService _customerService;
    private readonly IRoleService _roleService;
    private readonly IContactPreferenceService _contactPreferenceService;
    private FormValidator? _formValidator;


    [ObservableProperty]
    public CustomerRegistrationDto customerRegistrationDto;
    public ObservableCollection<RoleDto> AvailableRoles { get; private set; } = [];
    public ObservableCollection<ContactPreferenceDto> AvailableContactMethods { get; private set; } = [];

    public RegisterCustomerViewModel(IServiceProvider serviceProvider, ICustomerService customerService, IRoleService roleService, IContactPreferenceService contactPreferenceService)
    {
        _serviceProvider = serviceProvider;
        _customerService = customerService;
        _roleService = roleService;
        _contactPreferenceService = contactPreferenceService;
        
        CustomerRegistrationDto = new CustomerRegistrationDto
        {
            Address = new AddressDto(),
            Role = new RoleDto(),
            ContactPreference = new ContactPreferenceDto()
        };
    }

    [ObservableProperty]
    public string? password;

    [ObservableProperty]
    public string? confirmPassword;

    public bool ArePasswordsMatching => Password == ConfirmPassword;

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

    private void InitializeFormValidator()
    {
        if (CustomerRegistrationDto == null)
        {
            ShowMessage("Vänligen fyll i alla obligatoriska fält.");
        }
        else
        {
            _formValidator = new FormValidator(CustomerRegistrationDto, ShowMessage);
        }
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

    private void ClearForm()
    {
        CustomerRegistrationDto = new CustomerRegistrationDto();
        Password = string.Empty;
        ConfirmPassword = string.Empty;
    }

    private bool ValidatePasswords()
    {
        if (string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(ConfirmPassword))
        {
            ShowMessage("Du måste fylla i alla lösenordsfält.");
            return false;
        }

        if (Password != ConfirmPassword)
        {
            ShowMessage("Lösenorden matchar inte.");
            return false;
        }

        if (!IsValidPassword(Password))
        {
            ShowMessage("Lösenordet uppfyller inte kraven.");
            return false;
        }
        return true;
    }

    private static bool IsValidPassword(string password)
    {
        return Regex.IsMatch(password, @"^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*])(?=\S+$).{8,}$");
    }

    [RelayCommand]
    private async Task RegisterAsync()
    {
        if (!ValidatePasswords())
        {
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

        CustomerRegistrationDto.Password = Password!;

        var result = await _customerService.CreateCustomerAsync(CustomerRegistrationDto);

        if (result.IsSuccess)
        {
            ShowMessage("Registrering lyckades!");
            ClearForm();
        }
        else
        {
            ShowMessage(result.Message);
        }

    }

    [RelayCommand]
    private void NavigateToHome()
    {
        ClearForm();
        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = _serviceProvider.GetRequiredService<HomePageViewModel>();
    }
}
