using ApplicationCore.Business.Dtos;
using ApplicationCore.Business.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;


namespace PresentationAppWpf.Mvvm.ViewModels;

public partial class CustomerReviewViewModel : BaseViewModel
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ICustomerReviewService _customerReviewService;

    public CustomerReviewViewModel(IServiceProvider serviceProvider, ICustomerReviewService customerReviewService)
    {
        _serviceProvider = serviceProvider;
        _customerReviewService = customerReviewService;
        CustomerReviewDto = new CustomerReviewDto
        {
            Customer = new CustomerDto()
        };
    }

    [ObservableProperty]
    public CustomerReviewDto? customerReviewDto;
    [ObservableProperty]
    private ObservableCollection<CustomerReviewDto> reviews = [];

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
