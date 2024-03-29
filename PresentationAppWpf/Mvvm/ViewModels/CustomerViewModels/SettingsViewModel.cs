﻿using ApplicationCore.Business.Dtos;
using ApplicationCore.Business.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Windows;

namespace PresentationAppWpf.Mvvm.ViewModels;

public partial class SettingsViewModel(IServiceProvider serviceProvider, IRoleService roleService, IContactPreferenceService contactPreferenceService) : BaseViewModel
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly IRoleService _roleService = roleService;
    private readonly IContactPreferenceService _contactPreferenceService = contactPreferenceService;

    [ObservableProperty]
    public ObservableCollection<RoleDto> roles = [];
    private RoleDto? _selectedRole;
    public RoleDto SelectedRole
    {
        get => _selectedRole!;
        set => SetProperty(ref _selectedRole, value);
    }

    [ObservableProperty]
    public ObservableCollection<ContactPreferenceDto> contactPreferences = [];
    private ContactPreferenceDto? _selectedPreference;
    public ContactPreferenceDto SelectedPreference
    {
        get => _selectedPreference!;
        set => SetProperty(ref _selectedPreference, value);
    }

    [ObservableProperty]
    private RoleDto roleDto = new();

    [ObservableProperty]
    private ContactPreferenceDto contactPreferenceDto = new();

    [ObservableProperty]
    private bool isEditMode = false;

    private void ClearRoleForm()
    {
        RoleDto = new RoleDto();
    }

    private void ClearPreferenceForm()
    {
        ContactPreferenceDto = new ContactPreferenceDto();
    }

    [RelayCommand]
    private async Task AddRole()
    {
        var result = await _roleService.CreateRoleAsync(RoleDto);

        if (result != null)
        {
            if (result.IsSuccess)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Roles.Add(RoleDto);
                    ClearRoleForm();
                });
                await LoadRolesAsync();
                ShowMessage("Rollen har lagts till.");
            }
            else
            {
                ShowMessage("Det gick inte att lägga till rollen.");
            }
        }
    }

    private async Task LoadRolesAsync()
    {
        var result = await _roleService.GetAllRolesAsync();

        if (result != null && result.IsSuccess)
        {
            Roles = new ObservableCollection<RoleDto>(result.Data);
            OnPropertyChanged(nameof(Roles));
        }
        else
        {
            ShowMessage("Det finns inga roller i listan.");
        }
    }

    public async Task DeleteRoleAsync(int roleId)
    {
        var result = await _roleService.DeleteRoleAsync(roleId);

        if (result.IsSuccess)
        {
            var roleToRemove = Roles.First(r => r.Id == roleId);
            Roles.Remove(roleToRemove);
            ShowMessage("Rollen har tagits bort.");
        }
        else
        {
            ShowMessage("Rollen kunde inte raderas då den används av en eller flera kunder.");
        }
    }

    [RelayCommand]
    private async Task DeleteRole(int roleId)
    {
        await DeleteRoleAsync(roleId);
    }

    [RelayCommand]
    private void EditRole(RoleDto roleToEdit)
    {
        RoleDto = roleToEdit;
        IsEditMode = true;
    }

    [RelayCommand]
    private async Task SaveRole()
    {
        if (IsEditMode)
        {
            var updateResult = await _roleService.UpdateRoleAsync(RoleDto);
            if (updateResult.IsSuccess)
            {
                await LoadRolesAsync();
            }

            IsEditMode = false;
            SelectedRole = null!;
            ClearRoleForm();
        }
        else
        {
            await AddRole();
            ShowMessage("Rollen uppdaterades.");
        }
    }

    [RelayCommand]
    private async Task AddPreference()
    {
        var result = await _contactPreferenceService.CreateContactPreferenceAsync(ContactPreferenceDto);

        if (result != null)
        {
            if (result.IsSuccess)
            {
                ContactPreferences.Add(ContactPreferenceDto);
                ClearPreferenceForm();

                await LoadPreferencesAsync();
                ShowMessage("Preferensen har lagts till.");
            }
            else
            {
                ShowMessage("Det gick inte att lägga till preferensen.");
            }
        }
    }

    private async Task LoadPreferencesAsync()
    {
        var result = await _contactPreferenceService.GetAllContactPreferencesAsync();

        if (result != null && result.IsSuccess)
        {
            ContactPreferences = new ObservableCollection<ContactPreferenceDto>(result.Data);
            OnPropertyChanged(nameof(ContactPreferences));
        }
        else
        {
            ShowMessage("Det finns inga preferenser i listan.");
        }
    }

    public async Task DeletePreferenceAsync(int preferenceId)
    {
        var result = await _contactPreferenceService.DeleteContactPreferenceAsync(preferenceId);

        if (result.IsSuccess)
        {
            var preferenceToRemove = ContactPreferences.First(p => p.Id == preferenceId);
            ContactPreferences.Remove(preferenceToRemove);
            ShowMessage("Preferensen har tagits bort.");
        }
        else
        {
            ShowMessage("Preferensen kunde inte raderas då den används av en eller flera kunder.");
        }
    }

    [RelayCommand]
    private async Task DeletePreference(int preferenceId)
    {
        await DeletePreferenceAsync(preferenceId);
    }

    [RelayCommand]
    private void EditPreference(ContactPreferenceDto preferenceToEdit)
    {
        ContactPreferenceDto = preferenceToEdit;
        IsEditMode = true;
    }

    [RelayCommand]
    private async Task SavePreference()
    {
        if (IsEditMode)
        {
            var updateResult = await _contactPreferenceService.UpdateContactPreferenceAsync(ContactPreferenceDto);
            if (updateResult.IsSuccess)
            {
                await LoadPreferencesAsync();
            }
            IsEditMode = false;
            SelectedPreference = null!;
            ClearPreferenceForm();
        }
        else
        {
            await AddPreference();
            ShowMessage("Preferensen uppdaterades.");
        }
    }

    public async Task InitializeAsync()
    {
        await LoadRolesAsync();
        await LoadPreferencesAsync();
    }

    [RelayCommand]
    private void NavigateToHome()
    {
        ClearRoleForm();
        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = _serviceProvider.GetRequiredService<HomePageViewModel>();
    }
}
