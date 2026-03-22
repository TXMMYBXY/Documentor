using System.Windows.Input;

namespace Documentor.Presentation.Menu;

/// <summary>
/// Элемент меню
/// </summary>
public class NavigationMenuItem
{
    public string Title { get; set; } = string.Empty;
    public ICommand Command { get; set; } = null!;
}