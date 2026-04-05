using System.Windows;
using Documentor.Application.Services;
using Documentor.Core.Enums;
using Documentor.Presentation.Factories;
using Documentor.Presentation.ViewModels.Base;
using Documentor.Presentation.ViewModels.Windows;
using Documentor.Presentation.Views.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace Documentor.Infrastructure.Services;

public class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IPageViewModelFactory _pageViewModelFactory;

    private ViewModelBase? _currentPage;

    private readonly Dictionary<Type, Type> _viewToViewModelMapping;

    public ViewModelBase? CurrentPage => _currentPage;

    public event Action<ViewModelBase>? CurrentPageChanged;

    public NavigationService(
        IServiceProvider serviceProvider,
        IPageViewModelFactory pageViewModelFactory)
    {
        _serviceProvider = serviceProvider;
        _pageViewModelFactory = pageViewModelFactory;

        _viewToViewModelMapping = new Dictionary<Type, Type>
        {
            { typeof(LoginWindow), typeof(LoginWindowViewModel) },
            { typeof(MainShellWindow), typeof(MainShellViewModel) },

            // { typeof(CreateUserView), typeof(CreateUserViewModel) },
            // { typeof(ContractTemplateView), typeof(ContractTemplateViewModel) },
            // { typeof(StatementTemplateView), typeof(StatementTemplateViewModel) },
            // { typeof(CreateTemplateView), typeof(CreateTemplateDto) },
        };
    }

    public void NavigateToRole(int? roleId)
    {
        if (roleId is null)
            throw new InvalidOperationException("Роль пользователя не определена.");

        NavigateTo<MainShellWindow>();
    }

    public void NavigateTo<TView>() where TView : Window
    {
        var scope = _serviceProvider.CreateScope();

        try
        {
            var view = scope.ServiceProvider.GetRequiredService<TView>();

            if (_viewToViewModelMapping.TryGetValue(typeof(TView), out var viewModelType))
            {
                var viewModel = scope.ServiceProvider.GetService(viewModelType) as ViewModelBase;
                if (viewModel != null)
                {
                    view.DataContext = viewModel;
                }
            }

            view.Closed += (_, _) => scope.Dispose();

            ShowAsMainWindow(view);
        }
        catch
        {
            scope.Dispose();
            throw;
        }
    }

    public bool? ShowDialog<T>() where T : Window
    {
        var scope = _serviceProvider.CreateScope();

        try
        {
            var dialog = scope.ServiceProvider.GetRequiredService<T>();

            if (_viewToViewModelMapping.TryGetValue(typeof(T), out var viewModelType))
            {
                var viewModel = scope.ServiceProvider.GetService(viewModelType) as ViewModelBase;
                if (viewModel != null)
                {
                    dialog.DataContext = viewModel;

                    if (viewModel is IDialogService dialogAwareViewModel)
                    {
                        dialogAwareViewModel.DialogClosed += _ => dialog.Close();
                    }
                }
            }

            dialog.Owner = System.Windows.Application.Current.MainWindow;
            dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dialog.Closed += (_, _) => scope.Dispose();

            return dialog.ShowDialog();
        }
        catch
        {
            scope.Dispose();
            throw;
        }
    }

    public bool? ShowDialog<T>(ViewModelBase viewModel) where T : Window
    {
        var scope = _serviceProvider.CreateScope();

        try
        {
            var dialog = scope.ServiceProvider.GetRequiredService<T>();

            dialog.DataContext = viewModel;

            if (viewModel is IDialogService dialogAwareViewModel)
            {
                dialogAwareViewModel.DialogClosed += _ => dialog.Close();
            }

            dialog.Owner = System.Windows.Application.Current.MainWindow;
            dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dialog.Closed += (_, _) => scope.Dispose();

            return dialog.ShowDialog();
        }
        catch
        {
            scope.Dispose();
            throw;
        }
    }

    public void NavigateTo(PageKey pageKey)
    {
        _currentPage = _pageViewModelFactory.Create(pageKey);
        CurrentPageChanged?.Invoke(_currentPage);
    }

    private void ShowAsMainWindow(Window newWindow)
    {
        var currentWindow = System.Windows.Application.Current.MainWindow;

        System.Windows.Application.Current.MainWindow = newWindow;
        newWindow.Show();

        if (currentWindow != null && currentWindow != newWindow)
        {
            currentWindow.Close();
        }
    }
}