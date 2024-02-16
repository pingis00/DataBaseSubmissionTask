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

public partial class CreateProductViewModel : ObservableObject
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IProductService _productService;
    private readonly IBrandService _brandService;
    private readonly ICategoryService _categoryService;
    private readonly IInventoryService _inventoryService;
    private CreateProductValidator? _createProductValidator;

    [ObservableProperty]
    public CompleteProductDto completeProductDto;
    public ObservableCollection<BrandDto> AvailableBrands { get; private set; } = [];
    public ObservableCollection<CategoryDto> AvailableCategories { get; private set; } = [];

    public CreateProductViewModel(IServiceProvider serviceProvider, IProductService productService, IBrandService brandService, ICategoryService categoryService, IInventoryService inventoryService)
    {
        _serviceProvider = serviceProvider;
        _productService = productService;
        _brandService = brandService;
        _categoryService = categoryService;
        _inventoryService = inventoryService;

        CompleteProductDto = new CompleteProductDto
        {
            Brand = new BrandDto(),
            Category = new CategoryDto(),
            Inventory = new InventoryDto(),
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

    public async Task LoadBrandsAndCategoriesAsync()
    {
        var brandsResult = await _brandService.GetAllBrandsAsync();
        if (brandsResult.IsSuccess)
        {
            AvailableBrands.Clear();
            foreach (var brand in brandsResult.Data)
            {
                AvailableBrands.Add(brand);
            }
        }
        else
        {
            ShowMessage("Något gick fel");
        }

        var categoriesResult = await _categoryService.GetAllCategoriesAsync();
        if (categoriesResult.IsSuccess)
        {
            AvailableCategories.Clear();
            foreach (var preference in categoriesResult.Data)
            {
                AvailableCategories.Add(preference);
            }
        }
        else
        {
            ShowMessage("Något gick fel");
        }
    }

    private void ClearForm()
    {
        CompleteProductDto = new CompleteProductDto();
    }

    public void InitializeCreateProductValidator()
    {
        if (CompleteProductDto == null)
        {
            ShowMessage("Vänligen fyll i alla obligatoriska fält.");
        }
        else
        {
            _createProductValidator = new CreateProductValidator(CompleteProductDto, ShowMessage);
        }
    }

    [RelayCommand]
    private async Task CreateProductAsync()
    {
        InitializeCreateProductValidator();

        if (_createProductValidator == null)
        {
            return;
        }
        if (!_createProductValidator.ValidateProductCreation())
        {
            return;
        }

        var result = await _productService.CreateProductAsync(CompleteProductDto);

        if (result.IsSuccess)
        {
            ShowMessage("Produkten skapades!");
            ClearForm();
        }
        else
        {
            ShowMessage(result.Message);
        }

    }

    [RelayCommand]
    private void NavigateToHome()
    {
        ClearForm();
        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = _serviceProvider.GetRequiredService<HomePageViewModel>();
    }
}
