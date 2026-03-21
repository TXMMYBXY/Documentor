using DocumentFlowing.Presentation.ViewModels.Base;
using Documentor.Core.Enums;

namespace Documentor.Presentation.Navigation;

public interface INavigationService
{
    ViewModelBase? CurrentPage { get; }
    event Action<ViewModelBase>? CurrentPageChanged;
    void NavigateTo(PageKey pageKey);
}