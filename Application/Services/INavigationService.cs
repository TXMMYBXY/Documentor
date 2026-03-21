using System.Windows;
using DocumentFlowing.Presentation.ViewModels.Base;
using Documentor.Core.Enums;

namespace Documentor.Application.Services;

public interface INavigationService
{
    void NavigateToRole(int? roleId);

    void NavigateTo<TView>() where TView : Window;

    bool? ShowDialog<T>() where T : Window;

    bool? ShowDialog<T>(ViewModelBase viewModel) where T : Window;

    ViewModelBase? CurrentPage { get; }
    event Action<ViewModelBase>? CurrentPageChanged;
    void NavigateTo(PageKey pageKey);
}