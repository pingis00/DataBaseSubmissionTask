using ApplicationCore.Business.Dtos;
using ApplicationCore.ProductCatalog.Dtos;
using ApplicationCore.ProductCatalog.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;

namespace PresentationAppWpf.Mvvm.ViewModels.ProductViewModels;

public partial class UpdateProductReviewViewModel(IServiceProvider serviceProvider, IProductReviewService productReviewService) : ObservableObject
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly IProductReviewService _productReviewService = productReviewService;

    [ObservableProperty]
    private ProductReviewDto? productReviewDto;

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
    private async Task UpdateReview()
    {
        if (ProductReviewDto == null)
        {
            ShowMessage("Uppdateringsdata är inte korrekt inställd.");
            return;
        }

        var result = await _productReviewService.UpdateProductReviewAsync(ProductReviewDto);
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

    [RelayCommand]
    private async Task NavigateBackToReviewList()
    {
        var productReviewViewModel = _serviceProvider.GetRequiredService<ProductReviewViewModel>();
        await productReviewViewModel.LoadProductReviewsAsync();
        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = productReviewViewModel;
    }

}
