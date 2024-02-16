using ApplicationCore.Business.Dtos;
using ApplicationCore.ProductCatalog.Dtos;
using ApplicationCore.ProductCatalog.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;

namespace PresentationAppWpf.Mvvm.ViewModels.ProductViewModels;

public partial class ProductSettingsViewModel : ObservableObject
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IBrandService _brandService;
    private readonly ICategoryService _categoryService;

    public ProductSettingsViewModel(IServiceProvider serviceProvider, IBrandService brandService, ICategoryService categoryService)
    {
        _serviceProvider = serviceProvider;
        _brandService = brandService;
        _categoryService = categoryService;
    }

    [ObservableProperty]
    public ObservableCollection<BrandDto> brands = [];
    private BrandDto? _selectedBrand;

    public BrandDto SelectedBrand
    {
        get => _selectedBrand!;
        set => SetProperty(ref _selectedBrand, value);
    }

    [ObservableProperty]
    public ObservableCollection<CategoryDto> categories = [];
    private CategoryDto? _selectedCategory;

    public CategoryDto SelectedCategory
    {
        get => _selectedCategory!;
        set => SetProperty(ref _selectedCategory, value);
    }

    [ObservableProperty]
    private BrandDto brandDto = new();

    [ObservableProperty]
    private CategoryDto categoryDto = new();

    [ObservableProperty]
    private bool isEditMode = false;

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

    private void ClearBrandForm()
    {
        BrandDto = new BrandDto();
    }

    private void ClearCategoryForm()
    {
        CategoryDto = new CategoryDto();
    }

    [RelayCommand]
    private async Task AddBrand()
    {
        var result = await _brandService.CreateBrandAsync(BrandDto);

        if (result != null)
        {
            if (result.IsSuccess)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Brands.Add(BrandDto);
                    ClearBrandForm();
                });
                await LoadBrandsAsync();
                ShowMessage("Varumärket har lagts till.");
            }
            else
            {
                ShowMessage("Det gick inte att lägga till varumärket.");
            }
        }
    }

    [RelayCommand]
    private async Task AddCategory()
    {
        var result = await _categoryService.CreateCategoryAsync(CategoryDto);

        if (result != null)
        {
            if (result.IsSuccess)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Categories.Add(CategoryDto);
                    ClearCategoryForm();
                });
                await LoadCategoriesAsync();
                ShowMessage("Kategorin har lagts till.");
            }
            else
            {
                ShowMessage("Det gick inte att lägga till kategorin.");
            }
        }
    }

    private async Task LoadBrandsAsync()
    {
        var result = await _brandService.GetAllBrandsAsync();

        if (result != null && result.IsSuccess)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Brands = new ObservableCollection<BrandDto>(result.Data);
                OnPropertyChanged(nameof(Brands));
            });
        }
        else
        {
            ShowMessage("Det finns inga varumärken i listan.");
        }
    }

    private async Task LoadCategoriesAsync()
    {
        var result = await _categoryService.GetAllCategoriesAsync();

        if (result != null && result.IsSuccess)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Categories = new ObservableCollection<CategoryDto>(result.Data);
                OnPropertyChanged(nameof(Categories));
            });

        }
        else
        {
            ShowMessage("Det finns inga kategorier i listan.");
        }
    }

    public async Task DeleteBrandAsync(int brandId)
    {
        var result = await _brandService.DeleteBrandAsync(brandId);

        if (result.IsSuccess)
        {
            var brandToRemove = Brands.First(b => b.Id == brandId);
            Brands.Remove(brandToRemove);
            ShowMessage("Varumärket har tagits bort.");
        }
        else
        {
            ShowMessage("Det gick inte att ta bort varumärket.");
        }
    }

    public async Task DeleteCategoryAsync(int categoryId)
    {
        var result = await _categoryService.DeleteCategoryAsync(categoryId);

        Application.Current.Dispatcher.Invoke(() =>
        {
            if (result.IsSuccess)
            {
                var categoryToRemove = Categories.First(c => c.Id == categoryId);
                Categories.Remove(categoryToRemove);
                ShowMessage("Kategorin har tagits bort.");
            }
            else
            {
                ShowMessage("Det gick inte att ta bort kategorin.");
            }

        });
    }

    public async Task InitializeCategoryAndBrandAsync()
    {
        await LoadBrandsAsync();
        await LoadCategoriesAsync();
    }

    [RelayCommand]
    private async Task DeleteBrand(int brandId)
    {
        await DeleteBrandAsync(brandId);
    }

    [RelayCommand]
    private async Task DeleteCategory(int categoryId)
    {
        await DeleteCategoryAsync(categoryId);
    }

    [RelayCommand]
    private void EditBrand(BrandDto brandToEdit)
    {
        BrandDto = brandToEdit;
        IsEditMode = true;
    }

    [RelayCommand]
    private void EditCategory(CategoryDto categoryToEdit)
    {
        CategoryDto = categoryToEdit;
        IsEditMode = true;
    }

    [RelayCommand]
    private async Task SaveBrand()
    {
        if (IsEditMode)
        {
            var updateResult = await _brandService.UpdateBrandAsync(BrandDto);
            if (updateResult.IsSuccess)
            {
                await LoadBrandsAsync();
            }

            IsEditMode = false;
            SelectedBrand = null!;
            ClearBrandForm();
        }
        else
        {
            await AddBrand();
            ShowMessage("Varumärket har uppdaterades.");
        }
    }

    [RelayCommand]
    private async Task SaveCategory()
    {
        if (IsEditMode)
        {
            var updateResult = await _categoryService.UpdateCategoryAsync(CategoryDto);
            if (updateResult.IsSuccess)
            {
                await LoadCategoriesAsync();
            }
            IsEditMode = false;
            SelectedCategory = null!;
            ClearCategoryForm();
        }
        else
        {
            await AddCategory();
            ShowMessage("Kategorin uppdaterades.");
        }
    }

    [RelayCommand]
    private void NavigateToHome()
    {
        ClearBrandForm();
        ClearCategoryForm();
        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = _serviceProvider.GetRequiredService<HomePageViewModel>();
    }
}
