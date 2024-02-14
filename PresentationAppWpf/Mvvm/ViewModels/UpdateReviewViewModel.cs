using ApplicationCore.Business.Dtos;
using ApplicationCore.Business.Interfaces;
using ApplicationCore.Business.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using PresentationAppWpf.Validation;

namespace PresentationAppWpf.Mvvm.ViewModels;

public partial class UpdateReviewViewModel(IServiceProvider serviceProvider, ICustomerReviewService customerReviewService) : ObservableObject
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

    public void ShowMessage(string message)
    {
        MessageQueue.Enqueue(message);
    }

    [RelayCommand]
    private async Task NavigateBackToLReviewList()
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
            await NavigateBackToLReviewList();
            ShowMessage("Recensionen Uppdaterades.");
        }
        else
        {
            ShowMessage("Recensionuppdatering misslyckades.");
        }
    }
}
