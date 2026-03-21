using DocumentFlowing.Presentation.ViewModels.Base;
using Documentor.Core.Enums;
using Documentor.Presentation.Factories;

namespace Documentor.Presentation.Navigation;

public class NavigationService : INavigationService
{
    private readonly IPageViewModelFactory _pageViewModelFactory;
    private ViewModelBase? _currentPage;

    public ViewModelBase? CurrentPage => _currentPage;

    public event Action<ViewModelBase>? CurrentPageChanged;

    public NavigationService(IPageViewModelFactory pageViewModelFactory)
    {
        _pageViewModelFactory = pageViewModelFactory;
    }

    public void NavigateTo(PageKey pageKey)
    {
        _currentPage = _pageViewModelFactory.Create(pageKey);
        CurrentPageChanged?.Invoke(_currentPage);
    }
}