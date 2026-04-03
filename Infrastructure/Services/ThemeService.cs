using System.Windows;
using ControlzEx.Theming;
using Documentor.Core.Enums;
using Documentor.Core.Interfaces;

namespace Documentor.Infrastructure.Services;

public class ThemeService : IThemeService
{
    private const string LightBrushesPath = "Themes/AppBrushes.Light.xaml";
    private const string DarkBrushesPath = "Themes/AppBrushes.Dark.xaml";

    private readonly IAppSettingsService _appSettingsService;

    public AppTheme CurrentTheme { get; private set; } = AppTheme.Light;

    public ThemeService(IAppSettingsService appSettingsService)
    {
        _appSettingsService = appSettingsService;
    }

    public void ApplyTheme(AppTheme theme)
    {
        var app = System.Windows.Application.Current;
        if (app == null)
            return;

        var mahAppsTheme = theme == AppTheme.Dark
            ? "Dark.Blue"
            : "Light.Blue";

        ThemeManager.Current.ChangeTheme(app, mahAppsTheme);

        var dictionaries = app.Resources.MergedDictionaries;

        var existingBrushDictionary = dictionaries.FirstOrDefault(d =>
            d.Source != null &&
            (d.Source.OriginalString.EndsWith("AppBrushes.Light.xaml") ||
             d.Source.OriginalString.EndsWith("AppBrushes.Dark.xaml")));

        if (existingBrushDictionary != null)
        {
            dictionaries.Remove(existingBrushDictionary);
        }

        var brushPath = theme == AppTheme.Dark
            ? DarkBrushesPath
            : LightBrushesPath;

        dictionaries.Insert(3, new ResourceDictionary
        {
            Source = new Uri(brushPath, UriKind.Relative)
        });

        CurrentTheme = theme;
        _appSettingsService.SaveTheme(theme);
    }

    public void LoadSavedTheme()
    {
        var savedTheme = _appSettingsService.GetTheme();
        ApplyTheme(savedTheme);
    }
}