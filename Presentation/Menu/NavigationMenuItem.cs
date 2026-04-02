using System.Windows.Input;
using DocumentFlowing.Presentation.ViewModels.Base;
using Documentor.Core.Enums;
using MahApps.Metro.IconPacks;

namespace Documentor.Presentation.Menu;

/// <summary>
/// Элемент меню
/// </summary>
public class NavigationMenuItem : ViewModelBase
{
    private bool _isSelected;

    public string Title { get; set; } = string.Empty;

    public ICommand Command { get; set; } = null!;

    public PageKey PageKey { get; set; }

    public PackIconMaterialKind IconKind { get; set; }

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }
}