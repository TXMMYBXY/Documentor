using Microsoft.Win32;
using Documentor.Core.Enums;
using Documentor.Core.Interfaces;

namespace Documentor.Infrastructure.Services;

public class RegistryAppSettingsService : IAppSettingsService
{
    private const string RegistryPath = @"Software\Documentor\Settings";
    private const string ThemeValueName = "Theme";

    public AppTheme GetTheme()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(RegistryPath);

            var themeValue = key?.GetValue(ThemeValueName) as string;

            if (Enum.TryParse<AppTheme>(themeValue, out var theme))
            {
                return theme;
            }

            return AppTheme.Light;
        }
        catch
        {
            return AppTheme.Light;
        }
    }

    public void SaveTheme(AppTheme theme)
    {
        try
        {
            using var key = Registry.CurrentUser.CreateSubKey(RegistryPath);
            key?.SetValue(ThemeValueName, theme.ToString());
        }
        catch
        {
        }
    }
}