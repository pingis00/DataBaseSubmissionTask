using ApplicationCore.Business.Dtos;
using ApplicationCore.Business.Interfaces;
using ApplicationCore.Business.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
namespace PresentationAppWpf.Mvvm.ViewModels;

public partial class FullReviewViewModel(IServiceProvider serviceProvider, ICustomerReviewService customerReviewService) : ObservableObject
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ICustomerReviewService _customerReviewService = customerReviewService;

    [ObservableProperty]
    private CustomerReviewDto? customerReviewDto;


    private SnackbarMessageQueue _messageQueue = new(TimeSpan.FromSeconds(3));

    public SnackbarMessageQueue MessageQueue
    {
        get { return _messageQueue; }
        set { SetProperty(ref _messageQueue, value); }
    }

    [RelayCommand]
    private async Task NavigateToReviewList()
    {
        var customerReviewViewModel = _serviceProvider.GetRequiredService<CustomerReviewViewModel>();
        await customerReviewViewModel.LoadReviewsAsync();
        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = customerReviewViewModel;
    }
}
