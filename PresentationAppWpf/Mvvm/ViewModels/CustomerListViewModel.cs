using ApplicationCore.Business.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;

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
}
