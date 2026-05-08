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

    public IEnumerable<NavigationMenuItem> CreateForRole(Role role)
    {
        var items = new List<NavigationMenuItem>();

        switch (role)
        {
            case Role.Admin:
                items.Add(Create("Пользователи", PageKey.Users, PackIconMaterialKind.AccountGroupOutline));
                items.Add(Create("Отделы", PageKey.Departments, PackIconMaterialKind.Domain));
                items.Add(Create("Заявления", PageKey.StatementTemplates, PackIconMaterialKind.FileEditOutline));
                items.Add(Create("Архив", PageKey.Archive, PackIconMaterialKind.Archive));
                items.Add(Create("Профиль", PageKey.Profile, PackIconMaterialKind.AccountCircleOutline));
                items.Add(Create("Настройки", PageKey.Settings, PackIconMaterialKind.CogOutline));
                break;

            case Role.Boss:
                items.Add(Create("Шаблоны договоров", PageKey.ContractTemplates, PackIconMaterialKind.FileDocumentOutline));
                items.Add(Create("Шаблоны заявлений", PageKey.StatementTemplates, PackIconMaterialKind.FileEditOutline));
                items.Add(Create("Архив", PageKey.Archive, PackIconMaterialKind.Archive));
                items.Add(Create("Профиль", PageKey.Profile, PackIconMaterialKind.AccountCircleOutline));
                items.Add(Create("Настройки", PageKey.Settings, PackIconMaterialKind.CogOutline));
                break;

            case Role.Purchaser:
                items.Add(Create("Шаблоны договоров", PageKey.ContractTemplates, PackIconMaterialKind.FileDocumentOutline));
                items.Add(Create("Шаблоны заявлений", PageKey.StatementTemplates, PackIconMaterialKind.FileEditOutline));
                items.Add(Create("Архив", PageKey.Archive, PackIconMaterialKind.Archive));
                items.Add(Create("Профиль", PageKey.Profile, PackIconMaterialKind.AccountCircleOutline));
                items.Add(Create("Настройки", PageKey.Settings, PackIconMaterialKind.CogOutline));
                break;

            case Role.User:
                items.Add(Create("Шаблоны заявлений", PageKey.StatementTemplates, PackIconMaterialKind.FileEditOutline));
                items.Add(Create("Архив", PageKey.Archive, PackIconMaterialKind.Archive));
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