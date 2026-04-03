using Documentor.Core.Enums;

namespace Documentor.Core.Interfaces;

public interface IThemeService
{
    AppTheme CurrentTheme { get; }

    void ApplyTheme(AppTheme theme);
    void LoadSavedTheme();
}