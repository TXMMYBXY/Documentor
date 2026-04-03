using Documentor.Core.Enums;
using Documentor.Presentation.Menu;

namespace Documentor.Presentation.Factories;

/// <summary>
/// Сервис для создания функционала
/// </summary>
public interface IMenuFactory
{
    /// <summary>
    /// Создает функционал для роли
    /// </summary>
    /// <param name="role">Перечисление UserRole</param>
    /// <returns></returns>
    IEnumerable<NavigationMenuItem> CreateForRole(UserRole role);
}