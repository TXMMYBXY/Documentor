using DocumentFlowing.Common;
using Documentor.Core.Enums;
using Documentor.Presentation.Menu;
using Documentor.Presentation.Navigation;

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
                items.Add(Create("Панель", PageKey.Dashboard));
                items.Add(Create("Пользователи", PageKey.Users));
                items.Add(Create("Отделы", PageKey.Departments));
                items.Add(Create("Задачи", PageKey.Tasks));
                items.Add(Create("Профиль", PageKey.Profile));
                items.Add(Create("Настройки", PageKey.Settings));
                break;

            case UserRole.Boss:
                items.Add(Create("Панель", PageKey.Dashboard));
                items.Add(Create("Шаблоны договоров", PageKey.ContractTemplates));
                items.Add(Create("Шаблоны заявлений", PageKey.StatementTemplates));
                items.Add(Create("Задачи", PageKey.Tasks));
                items.Add(Create("Профиль", PageKey.Profile));
                items.Add(Create("Настройки", PageKey.Settings));
                break;

            case UserRole.Purchaser:
                items.Add(Create("Панель", PageKey.Dashboard));
                items.Add(Create("Шаблоны договоров", PageKey.ContractTemplates));
                items.Add(Create("Задачи", PageKey.Tasks));
                items.Add(Create("Профиль", PageKey.Profile));
                items.Add(Create("Настройки", PageKey.Settings));
                break;

            case UserRole.User:
                items.Add(Create("Панель", PageKey.Dashboard));
                items.Add(Create("Шаблоны заявлений", PageKey.StatementTemplates));
                items.Add(Create("Задачи", PageKey.Tasks));
                items.Add(Create("Профиль", PageKey.Profile));
                items.Add(Create("Настройки", PageKey.Settings));
                break;
        }

        return items;
    }

    private NavigationMenuItem Create(string title, PageKey pageKey)
    {
        return new NavigationMenuItem
        {
            Title = title,
            Command = new RelayCommand(() => _navigationService.NavigateTo(pageKey))
        };
    }
}