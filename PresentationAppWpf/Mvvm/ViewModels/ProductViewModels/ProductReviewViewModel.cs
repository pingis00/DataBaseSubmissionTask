using ApplicationCore.ProductCatalog.Dtos;
using ApplicationCore.ProductCatalog.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;

namespace PresentationAppWpf.Mvvm.ViewModels.ProductViewModels;

public partial class ProductReviewViewModel : BaseViewModel
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IProductReviewService _productReviewService;

    [ObservableProperty]
    public ProductReviewDto? productReviewDto;
    [ObservableProperty]
    private ObservableCollection<ProductReviewDto> productReviews = [];

    public ProductReviewViewModel(IServiceProvider serviceProvider, IProductReviewService productReviewService)
    {
        _serviceProvider = serviceProvider;
        _productReviewService = productReviewService;

        ProductReviewDto = new ProductReviewDto
        {
            Product = new ProductDto()
        };
    }

    public async Task LoadProductReviewsAsync()
    {
        var result = await _productReviewService.GetAllProductReviewsAsync();
        if (result.IsSuccess)
        {
            ProductReviews = new ObservableCollection<ProductReviewDto>(result.Data);
        }
        else
        {
            ShowMessage(result.Message);
        }
    }

    public async Task DeleteProductReviewAsync(int reviewId)
    {
        var result = await _productReviewService.DeleteProductReviewAsync(reviewId);

        if (result.IsSuccess)
        {
            var reviewToRemove = ProductReviews.First(r => r.Id == reviewId);
            ProductReviews.Remove(reviewToRemove);
        }
    }

    [RelayCommand]
    private async Task DeleteReview(int reviewId)
    {
        await DeleteProductReviewAsync(reviewId);
    }

    [RelayCommand]
    private async Task AddProductAsync()
    {
        var articleNumber = ProductReviewDto?.Product?.ArticleNumber;

        if (!articleNumber.HasValue)
        {
            ShowMessage("Artikelnummer måste anges.");
            return;
        }

        var createResult = await _productReviewService.CreateProductReviewAsync(ProductReviewDto!);
        if (createResult.IsSuccess)
        {
            ShowMessage("Din recension har lagts till!");
            ClearForm();
            await LoadProductReviewsAsync();
        }
        else
        {
            ShowMessage(createResult.Message);
        }
    }

    private void ClearForm()
    {
        ProductReviewDto = new ProductReviewDto();
    }

    [RelayCommand]
    private async Task NavigateToUpdateReview(int reviewId)
    {
        var reviewDtoResult = await _productReviewService.GetProductReviewByIdAsync(reviewId);
        if (!reviewDtoResult.IsSuccess || reviewDtoResult.Data == null)
        {
            ShowMessage("Kunde inte hämta Recensioninformation.");
            return;
        }

        var updateReviewViewModel = _serviceProvider.GetRequiredService<UpdateProductReviewViewModel>();
        updateReviewViewModel.ProductReviewDto = reviewDtoResult.Data;

        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = updateReviewViewModel;
    }

    [RelayCommand]
    private async Task NavigateToFullProductReview(int reviewId)
    {
        var reviewDtoResult = await _productReviewService.GetProductReviewByIdAsync(reviewId);
        if (!reviewDtoResult.IsSuccess || reviewDtoResult.Data == null)
        {
            ShowMessage("Kunde inte hämta Recensioninformation.");
            return;
        }

        var fullReviewViewModel = _serviceProvider.GetRequiredService<FullProductReviewViewModel>();
        fullReviewViewModel.ProductReviewDto = reviewDtoResult.Data;

        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = fullReviewViewModel;
    }
    [RelayCommand]
    private void NavigateHome()
    {
        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = _serviceProvider.GetRequiredService<HomePageViewModel>();
    }
}
