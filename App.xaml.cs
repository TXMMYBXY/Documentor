using Documentor.Core.Interfaces;
using Documentor.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace Documentor;

public partial class App : System.Windows.Application
{
    private ServiceProvider? _serviceProvider;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var services = new ServiceCollection();
            services.AddApplicationServices(configuration);

            _serviceProvider = services.BuildServiceProvider();

            var themeService = _serviceProvider.GetRequiredService<IThemeService>();
            themeService.LoadSavedTheme();

            var startupService = _serviceProvider.GetRequiredService<IAppStartupService>();
            await startupService.StartAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Ошибка запуска");
            Shutdown();
        }
    }
}