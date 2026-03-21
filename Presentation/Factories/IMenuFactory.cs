using Documentor.Core.Enums;
using Documentor.Presentation.Menu;

namespace Documentor.Presentation.Factories;

public interface IMenuFactory
{
    IEnumerable<NavigationMenuItem> CreateForRole(UserRole role);
}