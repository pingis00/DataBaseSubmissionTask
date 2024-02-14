using ApplicationCore.Business.Dtos;
using ApplicationCore.Business.Interfaces;
using ApplicationCore.Business.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using PresentationAppWpf.Validation;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Windows.Controls;

namespace PresentationAppWpf.Mvvm.ViewModels;

public partial class CustomerReviewViewModel : ObservableObject
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ICustomerReviewService _customerReviewService;
    private readonly ICustomerService _customerService;


    [ObservableProperty]
    public CustomerReviewDto? customerReviewDto;
    [ObservableProperty]
    private ObservableCollection<CustomerReviewDto> reviews = new ObservableCollection<CustomerReviewDto>();



    public CustomerReviewViewModel(IServiceProvider serviceProvider, ICustomerReviewService customerReviewService, ICustomerService customerService)
    {
        _serviceProvider = serviceProvider;
        _customerReviewService = customerReviewService;
        _customerService = customerService;
        CustomerReviewDto = new CustomerReviewDto
        {
            Customer = new CustomerDto()
        };
    }

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

    public async Task LoadReviewsAsync()
    {
        var result = await _customerReviewService.GetAllCustomerReviewsAsync();
        if (result.IsSuccess)
        {
            Reviews = new ObservableCollection<CustomerReviewDto>(result.Data);
        }
        else
        {
            ShowMessage(result.Message);
        }
    }

    public async Task DeleteReviewAsync(int reviewId)
    {
        var result = await _customerReviewService.DeleteCustomerReviewAsync(reviewId);

        if (result.IsSuccess)
        {
            var reviewToRemove = Reviews.First(r => r.Id == reviewId);
            Reviews.Remove(reviewToRemove);
        }
    }

    [RelayCommand]
    private async Task DeleteReview(int reviewId)
    {
        await DeleteReviewAsync(reviewId);
    }

    [RelayCommand]
    private async Task AddAsync()
    {
        string email = CustomerReviewDto?.Customer?.Email ?? string.Empty;
        if (string.IsNullOrWhiteSpace(email))
        {
            ShowMessage("E-postadressen måste anges.");
            return;
        }

        var createResult = await _customerReviewService.CreateCustomerReviewAsync(CustomerReviewDto!);
        if (createResult.IsSuccess)
        {
            ShowMessage("Din recension har lagts till!");
            ClearForm();
            await LoadReviewsAsync();
        }
        else
        {
            ShowMessage(createResult.Message);
        }
    }

    private void ClearForm()
    {
        CustomerReviewDto = new CustomerReviewDto();
    }

    [RelayCommand]
    private async Task NavigateToUpdateReview(int reviewId)
    {
        var reviewDtoResult = await _customerReviewService.GetCustomerReviewByIdAsync(reviewId);
        if (!reviewDtoResult.IsSuccess || reviewDtoResult.Data == null)
        {
            ShowMessage("Kunde inte hämta Recensioninformation.");
            return;
        }

        var updateReviewViewModel = _serviceProvider.GetRequiredService<UpdateReviewViewModel>();
        updateReviewViewModel.CustomerReviewDto = reviewDtoResult.Data;

        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = updateReviewViewModel;
    }

    [RelayCommand]
    private async Task NavigateToFullReview(int reviewId)
    {
        var reviewDtoResult = await _customerReviewService.GetCustomerReviewByIdAsync(reviewId);
        if (!reviewDtoResult.IsSuccess || reviewDtoResult.Data == null)
        {
            ShowMessage("Kunde inte hämta Recensioninformation.");
            return;
        }

        var fullReviewViewModel = _serviceProvider.GetRequiredService<FullReviewViewModel>();
        fullReviewViewModel.CustomerReviewDto = reviewDtoResult.Data;

        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = fullReviewViewModel;
    }

    [RelayCommand]
    private void NavigateHome()
    {
        ClearForm();
        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = _serviceProvider.GetRequiredService<HomePageViewModel>();
    }
}
