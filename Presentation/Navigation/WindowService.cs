using System.Windows;
using Documentor.Presentation.ViewModels.Windows;
using Documentor.Presentation.Views.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace Documentor.Presentation.Navigation;

public class WindowService : IWindowService
{
    private readonly IServiceProvider _serviceProvider;

    public WindowService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void ShowWindow<T>() where T : Window
    {
        var window = CreateWindow<T>();
        window.Show();
    }

    public void ReplaceMainWindow<T>() where T : Window
    {
        var currentApplication = System.Windows.Application.Current;
        var currentWindow = currentApplication?.MainWindow;
        var newWindow = CreateWindow<T>();

        if (currentApplication == null)
            throw new InvalidOperationException("WPF Application.Current is null.");

        currentApplication.MainWindow = newWindow;
        newWindow.Show();

        if (currentWindow != null && currentWindow != newWindow)
            currentWindow.Close();
    }

    private T CreateWindow<T>() where T : Window
    {
        var window = _serviceProvider.GetRequiredService<T>();

        switch (window)
        {
            case LoginWindow loginWindow:
                loginWindow.DataContext = _serviceProvider.GetRequiredService<LoginWindowViewModel>();
                break;

            case MainShellWindow mainShellWindow:
                mainShellWindow.DataContext = _serviceProvider.GetRequiredService<MainShellViewModel>();
                break;
        }

        return window;
    }
}