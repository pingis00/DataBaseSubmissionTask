using ApplicationCore.Infrastructure.Contexts;
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
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<MainWindow>();
            
            


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
