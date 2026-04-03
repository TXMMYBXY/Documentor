using Documentor.Core.Enums;
using Documentor.Presentation.ViewModels.Base;

namespace Documentor.Presentation.Navigation;

/// <summary>
/// Сервис для навигации страниц
/// </summary>
public interface INavigationService
{
    ViewModelBase? CurrentPage { get; }
    event Action<ViewModelBase>? CurrentPageChanged;
    void NavigateTo(PageKey pageKey);
}