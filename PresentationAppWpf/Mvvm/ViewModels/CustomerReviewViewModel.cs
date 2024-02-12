using ApplicationCore.Business.Dtos;
using ApplicationCore.Business.Interfaces;
using ApplicationCore.Business.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using PresentationAppWpf.Validation;
using System.Collections.ObjectModel;
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

    public async Task LoadReviews()
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

    [RelayCommand]
    private async Task AddAsync()
    {
        string email = CustomerReviewDto?.Customer?.Email ?? string.Empty;
        if (string.IsNullOrWhiteSpace(email))
        {
            ShowMessage("E-postadressen måste anges.");
            return;
        }

        var customerResult = await _customerService.GetCustomerByEmailAsync(email);
        if (!customerResult.IsSuccess)
        {
            ShowMessage("Ingen kund med angiven e-postadress hittades.");
            return;
        }


        var createResult = await _customerReviewService.CreateCustomerReviewAsync(CustomerReviewDto!);
        if (createResult.IsSuccess)
        {
            ShowMessage("Din recension har lagts till!");
            ClearForm();
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
    private void NavigateHome()
    {
        ClearForm();
        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = _serviceProvider.GetRequiredService<HomePageViewModel>();
    }
}
