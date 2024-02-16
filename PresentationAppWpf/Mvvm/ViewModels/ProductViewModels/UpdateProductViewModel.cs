using ApplicationCore.Business.Dtos;
using ApplicationCore.ProductCatalog.Dtos;
using ApplicationCore.ProductCatalog.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using PresentationAppWpf.Validation;
using System.Collections.ObjectModel;

namespace PresentationAppWpf.Mvvm.ViewModels.ProductViewModels;

public partial class UpdateProductViewModel(IServiceProvider serviceProvider, IProductService productService, IBrandService brandService, ICategoryService categoryService, CreateProductValidator? createProductValidator) : ObservableObject
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly IProductService _productService = productService;
    private readonly IBrandService _brandService = brandService;
    private readonly ICategoryService _categoryService = categoryService;
    private CreateProductValidator? _createProductValidator = createProductValidator;

    [ObservableProperty]
    private UpdateProductDto? updateProductDto;
    public ObservableCollection<BrandDto> AvailableBrands { get; private set; } = [];
    public ObservableCollection<CategoryDto> AvailableCategories { get; private set; } = [];

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

    public async Task InitializeBrandsAndCategories()
    {
        await LoadBrandsAndCategoriesAsync();
    }

    public async Task LoadBrandsAndCategoriesAsync()
    {
        var brandResult = await _brandService.GetAllBrandsAsync();
        if (brandResult.IsSuccess)
        {
            AvailableBrands.Clear();
            foreach (var brand in brandResult.Data)
            {
                AvailableBrands.Add(brand);
            }
        }
        else
        {
            ShowMessage("Något gick fel");
        }

        var CategoryResult = await _categoryService.GetAllCategoriesAsync();
        if (CategoryResult.IsSuccess)
        {
            AvailableCategories.Clear();
            foreach (var category in CategoryResult.Data)
            {
                AvailableCategories.Add(category);
            }
        }
        else
        {
            ShowMessage("Något gick fel");
        }
    }

    public void InitializeCreateProductValidator()
    {
        if (UpdateProductDto == null)
        {
            ShowMessage("Vänligen fyll i alla obligatoriska fält.");
        }
        else
        {
            _createProductValidator = new CreateProductValidator(UpdateProductDto, ShowMessage);
        }
    }

    [RelayCommand]
    private async Task UpdateProduct()
    {
        if (UpdateProductDto == null)
        {
            ShowMessage("Uppdateringsdata är inte korrekt inställd.");
            return;
        }

        InitializeCreateProductValidator();

        if (_createProductValidator == null)
        {
            return;
        }
        if (!_createProductValidator.ValidateProductCreation())
        {
            return;
        }

        var result = await _productService.UpdateProductAsync(UpdateProductDto);
        if (result.IsSuccess)
        {
            ShowMessage("Kunduppdatering lyckades.");
            await NavigateBackToProductView();
        }
        else
        {
            ShowMessage("Kunduppdatering misslyckades.");
        }
    }

    private async Task NavigateBackToProductView()
    {
        var productListViewModel = _serviceProvider.GetRequiredService<ProductListViewModel>();
        await productListViewModel.LoadProductsAsync();
        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = productListViewModel;
    }

}
