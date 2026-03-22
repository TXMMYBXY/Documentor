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
                items.Add(_Create("Панель", PageKey.Dashboard));
                items.Add(_Create("Пользователи", PageKey.Users));
                items.Add(_Create("Отделы", PageKey.Departments));
                items.Add(_Create("Профиль", PageKey.Profile));
                items.Add(_Create("Настройки", PageKey.Settings));
                break;

            case UserRole.Boss:
                items.Add(_Create("Панель", PageKey.Dashboard));
                items.Add(_Create("Шаблоны договоров", PageKey.ContractTemplates));
                items.Add(_Create("Шаблоны заявлений", PageKey.StatementTemplates));
                items.Add(_Create("Задачи", PageKey.Tasks));
                items.Add(_Create("Профиль", PageKey.Profile));
                items.Add(_Create("Настройки", PageKey.Settings));
                break;

            case UserRole.Purchaser:
                items.Add(_Create("Панель", PageKey.Dashboard));
                items.Add(_Create("Шаблоны договоров", PageKey.ContractTemplates));
                items.Add(_Create("Задачи", PageKey.Tasks));
                items.Add(_Create("Профиль", PageKey.Profile));
                items.Add(_Create("Настройки", PageKey.Settings));
                break;

            case UserRole.User:
                items.Add(_Create("Панель", PageKey.Dashboard));
                items.Add(_Create("Шаблоны заявлений", PageKey.StatementTemplates));
                items.Add(_Create("Задачи", PageKey.Tasks));
                items.Add(_Create("Профиль", PageKey.Profile));
                items.Add(_Create("Настройки", PageKey.Settings));
                break;
        }

        return items;
    }

    private NavigationMenuItem _Create(string title, PageKey pageKey)
    {
        return new NavigationMenuItem
        {
            Title = title,
            Command = new RelayCommand(() => _navigationService.NavigateTo(pageKey))
        };
    }
}