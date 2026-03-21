using System.Windows.Input;

namespace Documentor.Presentation.Menu;

public class NavigationMenuItem
{
    public string Title { get; set; } = string.Empty;
    public ICommand Command { get; set; } = null!;
}