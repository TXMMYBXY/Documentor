using System.Collections.ObjectModel;
using System.Windows.Input;
using DocumentFlowing.Common;
using Documentor.Common;
using Documentor.Core.Enums;
using Documentor.Core.Interfaces;
using Documentor.Presentation.Factories;
using Documentor.Presentation.Menu;
using Documentor.Presentation.Navigation;
using Documentor.Presentation.Services;
using Documentor.Presentation.ViewModels.Base;
using Documentor.Presentation.ViewModels.Pages;

namespace Documentor.Presentation.ViewModels.Windows;

public class MainShellViewModel : ViewModelBase
{
    private readonly IUserSession _userSession;
    private readonly INavigationService _navigationService;
    private readonly IApplicationNavigationService _applicationNavigationService;

    private ViewModelBase? _currentPage;
    private bool _isNavigationPaneExpanded = true;

    public string Title => _userSession.Role switch
    {
        UserRole.Admin => "Админ панель",
        UserRole.Boss => "Панель начальника",
        UserRole.Purchaser => "Панель сотрудника отдела закупок",
        UserRole.User => "Панель сотрудника",
        _ => "ошибка"
    };

    public ObservableCollection<NavigationMenuItem> MenuItems { get; } = new();

    public ViewModelBase? CurrentPage
    {
        get => _currentPage;
        set => SetProperty(ref _currentPage, value);
    }

    public bool IsNavigationPaneExpanded
    {
        get => _isNavigationPaneExpanded;
        set => SetProperty(ref _isNavigationPaneExpanded, value);
    }

    public ICommand LogoutCommand { get; }
    public ICommand ToggleNavigationPaneCommand { get; }
    public IInAppToastSource Toasts { get; }

    public MainShellViewModel(
        IUserSession userSession,
        INavigationService navigationService,
        IMenuFactory menuFactory,
        IApplicationNavigationService applicationNavigationService,
        IInAppToastSource toasts)
    {
        _userSession = userSession;
        _navigationService = navigationService;
        _applicationNavigationService = applicationNavigationService;
        Toasts = toasts;

        LogoutCommand = new RelayCommand(() => _applicationNavigationService.Logout());
        ToggleNavigationPaneCommand = new RelayCommand(() => IsNavigationPaneExpanded = !IsNavigationPaneExpanded);

        foreach (var item in menuFactory.CreateForRole(_userSession.Role))
            MenuItems.Add(item);

        _navigationService.CurrentPageChanged += OnCurrentPageChanged;

        var defaultPage = GetDefaultPage(_userSession.Role);
        SelectMenuItem(defaultPage);
        _navigationService.NavigateTo(defaultPage);
    }

    private void OnCurrentPageChanged(ViewModelBase viewModel)
    {
        CurrentPage = viewModel;

        var pageKey = ResolvePageKey(viewModel);
        if (pageKey.HasValue)
        {
            SelectMenuItem(pageKey.Value);
        }
    }

    private void SelectMenuItem(PageKey pageKey)
    {
        foreach (var item in MenuItems)
        {
            item.IsSelected = item.PageKey == pageKey;
        }
    }

    private static PageKey? ResolvePageKey(ViewModelBase viewModel)
    {
        return viewModel switch
        {
            DashboardPageViewModel => PageKey.Dashboard,
            UsersPageViewModel => PageKey.Users,
            DepartmentsPageViewModel => PageKey.Departments,
            ContractTemplatesPageViewModel => PageKey.ContractTemplates,
            StatementTemplatesPageViewModel => PageKey.StatementTemplates,
            TasksPageViewModel => PageKey.Tasks,
            ProfilePageViewModel => PageKey.Profile,
            SettingsPageViewModel => PageKey.Settings,
            _ => null
        };
    }

    private static PageKey GetDefaultPage(UserRole role)
    {
        return role switch
        {
            UserRole.Admin => PageKey.Users,
            UserRole.Boss => PageKey.ContractTemplates,
            UserRole.Purchaser => PageKey.ContractTemplates,
            UserRole.User => PageKey.StatementTemplates,
            _ => PageKey.Dashboard
        };
    }
}