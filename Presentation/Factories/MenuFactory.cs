using DocumentFlowing.Common;
using Documentor.Common;
using Documentor.Core.Enums;
using Documentor.Presentation.Menu;
using Documentor.Presentation.Navigation;
using MahApps.Metro.IconPacks;

namespace Documentor.Presentation.Factories;

public class MenuFactory : IMenuFactory
{
    private readonly INavigationService _navigationService;

    public MenuFactory(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    public IEnumerable<NavigationMenuItem> CreateForRole(UserRole role)
    {
        var items = new List<NavigationMenuItem>();

        switch (role)
        {
            case UserRole.Admin:
                items.Add(Create("Панель", PageKey.Dashboard, PackIconMaterialKind.ViewDashboardOutline));
                items.Add(Create("Пользователи", PageKey.Users, PackIconMaterialKind.AccountGroupOutline));
                items.Add(Create("Отделы", PageKey.Departments, PackIconMaterialKind.Domain));
                items.Add(Create("Заявления", PageKey.StatementTemplates, PackIconMaterialKind.FileEditOutline));
                items.Add(Create("Профиль", PageKey.Profile, PackIconMaterialKind.AccountCircleOutline));
                items.Add(Create("Настройки", PageKey.Settings, PackIconMaterialKind.CogOutline));
                break;

            case UserRole.Boss:
                items.Add(Create("Панель", PageKey.Dashboard, PackIconMaterialKind.ViewDashboardOutline));
                items.Add(Create("Шаблоны договоров", PageKey.ContractTemplates, PackIconMaterialKind.FileDocumentOutline));
                items.Add(Create("Шаблоны заявлений", PageKey.StatementTemplates, PackIconMaterialKind.FileEditOutline));
                items.Add(Create("Задачи", PageKey.Tasks, PackIconMaterialKind.FormatListChecks));
                items.Add(Create("Профиль", PageKey.Profile, PackIconMaterialKind.AccountCircleOutline));
                items.Add(Create("Настройки", PageKey.Settings, PackIconMaterialKind.CogOutline));
                break;

            case UserRole.Purchaser:
                items.Add(Create("Панель", PageKey.Dashboard, PackIconMaterialKind.ViewDashboardOutline));
                items.Add(Create("Шаблоны договоров", PageKey.ContractTemplates, PackIconMaterialKind.FileDocumentOutline));
                items.Add(Create("Шаблоны заявлений", PageKey.StatementTemplates, PackIconMaterialKind.FileEditOutline));
                items.Add(Create("Профиль", PageKey.Profile, PackIconMaterialKind.AccountCircleOutline));
                items.Add(Create("Настройки", PageKey.Settings, PackIconMaterialKind.CogOutline));
                break;

            case UserRole.User:
                items.Add(Create("Панель", PageKey.Dashboard, PackIconMaterialKind.ViewDashboardOutline));
                items.Add(Create("Шаблоны заявлений", PageKey.StatementTemplates, PackIconMaterialKind.FileEditOutline));
                items.Add(Create("Задачи", PageKey.Tasks, PackIconMaterialKind.FormatListChecks));
                items.Add(Create("Профиль", PageKey.Profile, PackIconMaterialKind.AccountCircleOutline));
                items.Add(Create("Настройки", PageKey.Settings, PackIconMaterialKind.CogOutline));
                break;
        }

        return items;
    }

    private NavigationMenuItem Create(string title, PageKey pageKey, PackIconMaterialKind iconKind)
    {
        return new NavigationMenuItem
        {
            Title = title,
            PageKey = pageKey,
            IconKind = iconKind,
            Command = new RelayCommand(() => _navigationService.NavigateTo(pageKey))
        };
    }
}