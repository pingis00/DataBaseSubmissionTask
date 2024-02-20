using ApplicationCore.Business.Dtos;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
namespace PresentationAppWpf.Mvvm.ViewModels;

public partial class FullReviewViewModel(IServiceProvider serviceProvider) : BaseViewModel
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    [ObservableProperty]
    private CustomerReviewDto? customerReviewDto;

    [RelayCommand]
    private async Task NavigateToReviewList()
    {
        var customerReviewViewModel = _serviceProvider.GetRequiredService<CustomerReviewViewModel>();
        await customerReviewViewModel.LoadReviewsAsync();
        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = customerReviewViewModel;
    }
}
