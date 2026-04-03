using Microsoft.Win32;
using Documentor.Core.Enums;
using Documentor.Core.Interfaces;

namespace Documentor.Infrastructure.Services;

public class RegistryAppSettingsService : IAppSettingsService
{
    private const string RegistryPath = @"Software\Documentor\Settings";
    private const string ThemeValueName = "Theme";
    private const string PageSizeValueName = "PageSize";
    private const string ApiDomainOverrideValueName = "ApiDomainOverride";

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

    public int GetPageSize()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(RegistryPath);

            var value = key?.GetValue(PageSizeValueName);

            if (value is int pageSize && pageSize > 0)
                return pageSize;

            if (int.TryParse(value?.ToString(), out var parsed) && parsed > 0)
                return parsed;

            return 10;
        }
        catch
        {
            return 10;
        }
    }

    public void SavePageSize(int pageSize)
    {
        try
        {
            if (pageSize <= 0)
                pageSize = 10;

            using var key = Registry.CurrentUser.CreateSubKey(RegistryPath);
            key?.SetValue(PageSizeValueName, pageSize, RegistryValueKind.DWord);
        }
        catch
        {
        }
    }

    public string? GetApiDomainOverride()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(RegistryPath);
            return key?.GetValue(ApiDomainOverrideValueName) as string;
        }
        catch
        {
            return null;
        }
    }

    public void SaveApiDomainOverride(string domain)
    {
        try
        {
            using var key = Registry.CurrentUser.CreateSubKey(RegistryPath);
            key?.SetValue(ApiDomainOverrideValueName, domain);
        }
        catch
        {
        }
    }

    public void ClearApiDomainOverride()
    {
        try
        {
            using var key = Registry.CurrentUser.CreateSubKey(RegistryPath);
            key?.DeleteValue(ApiDomainOverrideValueName, false);
        }
        catch
        {
        }
    }
}