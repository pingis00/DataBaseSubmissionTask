using ApplicationCore.Business.Dtos;
using ApplicationCore.Business.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace PresentationAppWpf.Mvvm.ViewModels;

public partial class UpdateReviewViewModel(IServiceProvider serviceProvider, ICustomerReviewService customerReviewService) : BaseViewModel
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ICustomerReviewService _customerReviewService = customerReviewService;

    [ObservableProperty]
    private CustomerReviewDto? customerReviewDto;

    [RelayCommand]
    private async Task NavigateBackToReviewList()
    {
        var customerReviewViewModel = _serviceProvider.GetRequiredService<CustomerReviewViewModel>();
        await customerReviewViewModel.LoadReviewsAsync();
        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = customerReviewViewModel;
    }

    [RelayCommand]
    private async Task UpdateReview()
    {
        if (CustomerReviewDto == null)
        {
            ShowMessage("Uppdateringsdata är inte korrekt inställd.");
            return;
        }

        var result = await _customerReviewService.UpdateCustomerReviewAsync(CustomerReviewDto);
        if (result.IsSuccess)
        {
            await NavigateBackToReviewList();
            ShowMessage("Recensionen Uppdaterades.");
        }
        else
        {
            ShowMessage("Recensionuppdatering misslyckades.");
        }
    }

}
