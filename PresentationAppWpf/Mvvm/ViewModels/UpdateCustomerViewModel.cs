using ApplicationCore.Business.Dtos;
using ApplicationCore.Business.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace PresentationAppWpf.Mvvm.ViewModels;

public partial class UpdateCustomerViewModel : ObservableObject
{
    [ObservableProperty]
    private UpdateCustomerDto updateCustomerDto;
}
