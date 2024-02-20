using ApplicationCore.Business.Dtos;
using ApplicationCore.Business.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;

namespace PresentationAppWpf.Mvvm.ViewModels.CustomerViewModels;

public partial class AddressListViewModel(IServiceProvider serviceProvider, IAddressService addressService) : BaseViewModel
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly IAddressService _addressService = addressService;

    [ObservableProperty]
    public ObservableCollection<AddressDto> addresses = [];
    private AddressDto? _selectedAddress;
    public AddressDto SelectedAddress
    {
        get => _selectedAddress!;
        set => SetProperty(ref _selectedAddress, value);
    }

    public async Task LoadAddressesAsync()
    {
        var result = await _addressService.GetAllAddressesAsync();

        if (result != null && result.IsSuccess)
        {
            Addresses = new ObservableCollection<AddressDto>(result.Data);
            OnPropertyChanged(nameof(Addresses));
        }
        else
        {
            ShowMessage("Addrresslistan är tom.");
        }
    }

    public async Task DeleteAddressAsync(int addressId)
    {
        var result = await _addressService.DeleteAddressAsync(addressId);

        if (result.IsSuccess)
        {
            var addressToRemove = Addresses.First(a => a.Id == addressId);
            Addresses.Remove(addressToRemove);
            ShowMessage("Adressen raderades");
        }
        else
        {
            ShowMessage("Adressen kunde inte raderas då den används av en eller flera kunder.");
        }
    }

    [RelayCommand]
    private async Task DeleteAddress(int customerId)
    {
        await DeleteAddressAsync(customerId);
    }

    [RelayCommand]
    private void NavigateBack()
    {
        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = _serviceProvider.GetRequiredService<HomePageViewModel>();
    }
}
