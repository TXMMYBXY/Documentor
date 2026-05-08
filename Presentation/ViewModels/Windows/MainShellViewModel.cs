using System.Collections.ObjectModel;
using System.Windows.Input;
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
        Role.Admin => "Админ панель",
        Role.Boss => "Панель начальника",
        Role.Purchaser => "Панель сотрудника отдела закупок",
        Role.User => "Панель сотрудника",
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
            UsersPageViewModel => PageKey.Users,
            DepartmentsPageViewModel => PageKey.Departments,
            TemplatesPageViewModel templates => templates.PageKey,
            DocumentPageViewModel => PageKey.Archive,
            ProfilePageViewModel => PageKey.Profile,
            SettingsPageViewModel => PageKey.Settings,
            _ => null
        };
    }

    private static PageKey GetDefaultPage(Role role)
    {
        return role switch
        {
            Role.Admin => PageKey.Users,
            Role.Boss => PageKey.ContractTemplates,
            Role.Purchaser => PageKey.StatementTemplates,
            Role.User => PageKey.Archive,
            _ => PageKey.Settings
        };
    }
}