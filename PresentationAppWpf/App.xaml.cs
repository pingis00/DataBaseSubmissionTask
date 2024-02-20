using ApplicationCore.Business.Interfaces;
using ApplicationCore.Business.Services;
using ApplicationCore.Infrastructure.Contexts;
using ApplicationCore.Infrastructure.Interfaces;
using ApplicationCore.Infrastructure.Repositories;
using ApplicationCore.ProductCatalog.Context;
using ApplicationCore.ProductCatalog.Interfaces;
using ApplicationCore.ProductCatalog.Repositories;
using ApplicationCore.ProductCatalog.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PresentationAppWpf.Mvvm.ViewModels;
using PresentationAppWpf.Mvvm.ViewModels.CustomerViewModels;
using PresentationAppWpf.Mvvm.ViewModels.ProductViewModels;
using PresentationAppWpf.Mvvm.Views;
using PresentationAppWpf.Mvvm.Views.CustomerViews;
using PresentationAppWpf.Mvvm.Views.ProductViews;
using System.Windows;

namespace PresentationAppWpf;


public partial class App : Application
{
    private IHost? AppHost { get; set; }

    public App()
    {
        AppHost = Host.CreateDefaultBuilder().ConfigureServices(services =>
        {
            
            services.AddDbContext<EagerLoadingContext>(x => x.UseSqlServer(@"Data Source=localhost;Initial Catalog=DbCustomers;Integrated Security=True;Encrypt=True;Trust Server Certificate=True", x => x.MigrationsAssembly(nameof(ApplicationCore))));
            services.AddDbContext<DataContext>(x => x.UseSqlServer(@"Data Source=localhost;Initial Catalog=ProductsCatalog;Integrated Security=True;Trust Server Certificate=True", x => x.MigrationsAssembly(nameof(ApplicationCore))));
        

            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();

            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<IAddressRepository, AddressRepository>();

            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IRoleRepository, RoleRepository>();

            services.AddScoped<IContactPreferenceService, ContactPreferenceService>();
            services.AddScoped<IContactPreferenceRepository, ContactPreferenceRepository>();

            services.AddScoped<ICustomerReviewService, CustomerReviewService>();
            services.AddScoped<ICustomerReviewRepository, CustomerReviewRepository>();

            services.AddScoped<IBrandService, BrandService>();
            services.AddScoped<IBrandRepository, BrandRepository>();

            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();

            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IProductRepository, ProductRepository>();

            services.AddScoped<IInventoryService, InventoryService>();
            services.AddScoped<IIventoryRepository, InventoryRepository>();

            services.AddScoped<IProductReviewService, ProductReviewService>();
            services.AddScoped<IProductReviewRepository, ProductReviewRepository>();

            services.AddSingleton<MainViewModel>();
            services.AddSingleton<MainWindow>();

            services.AddTransient<HomePageViewModel>();
            services.AddTransient<HomePageView>();

            services.AddTransient<RegisterCustomerViewModel>();
            services.AddTransient<RegisterCustomerView>();

            services.AddTransient<CustomerListViewModel>();
            services.AddTransient<CustomerListView>();

            services.AddTransient<UpdateCustomerViewModel>();
            services.AddTransient<UpdateCustomerView>();

            services.AddTransient<CustomerReviewViewModel>();
            services.AddTransient<CustomerReviewView>();

            services.AddTransient<SettingsViewModel>();
            services.AddTransient<SettingsView>();

            services.AddTransient<UpdateReviewViewModel>();
            services.AddTransient<UpdateReviewView>();

            services.AddTransient<FullReviewViewModel>();
            services.AddTransient<FullReviewView>();

            services.AddTransient<ProductSettingsViewModel>();
            services.AddTransient<ProductSettingsView>();

            services.AddTransient<CreateProductViewModel>();
            services.AddTransient<CreateProductView>();

            services.AddTransient<ProductListViewModel>();
            services.AddTransient<ProductListView>();

            services.AddTransient<UpdateProductViewModel>();
            services.AddTransient<UpdateProductView>();

            services.AddTransient<ProductReviewViewModel>();
            services.AddTransient<ProductReviewView>();

            services.AddTransient<UpdateProductReviewViewModel>();
            services.AddTransient<UpdateProductReviewView>();

            services.AddTransient<FullProductReviewViewModel>();
            services.AddTransient<FullProductReviewView>();

            services.AddTransient<ProductInventoryViewModel>();
            services.AddTransient<ProductInventoryView>();

            services.AddTransient<UpdateInventoryViewModel>();
            services.AddTransient<UpdateInventoryView>();

            services.AddTransient<FullProductViewModel>();
            services.AddTransient<FullProductView>();

            services.AddTransient<AddressListViewModel>();
            services.AddTransient<AddressListView>();

        }).Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await AppHost!.StartAsync();

        var mainWindow = AppHost!.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();

        base.OnStartup(e);
    }
}
