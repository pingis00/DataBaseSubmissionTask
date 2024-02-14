using ApplicationCore.Business.Interfaces;
using ApplicationCore.Business.Services;
using ApplicationCore.Infrastructure.Contexts;
using ApplicationCore.Infrastructure.Interfaces;
using ApplicationCore.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PresentationAppWpf.Mvvm.ViewModels;
using PresentationAppWpf.Mvvm.Views;
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
