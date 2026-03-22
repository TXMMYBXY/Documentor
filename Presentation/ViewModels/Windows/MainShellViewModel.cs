using System.Collections.ObjectModel;
using System.Windows.Input;
using DocumentFlowing.Common;
using DocumentFlowing.Presentation.ViewModels.Base;
using Documentor.Core.Enums;
using Documentor.Core.Interfaces;
using Documentor.Presentation.Factories;
using Documentor.Presentation.Menu;
using Documentor.Presentation.Navigation;

namespace Documentor.Presentation.ViewModels.Windows;

public class MainShellViewModel : ViewModelBase
{
    private readonly IUserSession _userSession;
    private readonly INavigationService _navigationService;
    private readonly IApplicationNavigationService _applicationNavigationService;

    private ViewModelBase? _currentPage;

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

    public ICommand LogoutCommand { get; }

    public MainShellViewModel(
        IUserSession userSession,
        INavigationService navigationService,
        IMenuFactory menuFactory,
        IApplicationNavigationService applicationNavigationService)
    {
        _userSession = userSession;
        _navigationService = navigationService;
        _applicationNavigationService = applicationNavigationService;

        LogoutCommand = new RelayCommand(() => _applicationNavigationService.Logout());

        foreach (var item in menuFactory.CreateForRole(_userSession.Role))
            MenuItems.Add(item);

        _navigationService.CurrentPageChanged += OnCurrentPageChanged;

        var defaultPage = GetDefaultPage(_userSession.Role);
        _navigationService.NavigateTo(defaultPage);
    }

    private void OnCurrentPageChanged(ViewModelBase viewModel)
    {
        CurrentPage = viewModel;
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