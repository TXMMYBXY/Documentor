using System.Windows;
using Documentor.Core.Enums;
using Documentor.Core.Interfaces;

namespace Documentor.Infrastructure.Services;

public class ThemeService : IThemeService
{
    private const string LightThemePath = "Themes/LightTheme.xaml";
    private const string DarkThemePath = "Themes/DarkTheme.xaml";

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

        var dictionaries = app.Resources.MergedDictionaries;

        var existingTheme = dictionaries
            .FirstOrDefault(d =>
                d.Source != null &&
                (d.Source.OriginalString.EndsWith("LightTheme.xaml") ||
                 d.Source.OriginalString.EndsWith("DarkTheme.xaml")));

        if (existingTheme != null)
        {
            dictionaries.Remove(existingTheme);
        }

        var themePath = theme == AppTheme.Light
            ? LightThemePath
            : DarkThemePath;

        var newTheme = new ResourceDictionary
        {
            Source = new Uri(themePath, UriKind.Relative)
        };

        dictionaries.Insert(0, newTheme);

        CurrentTheme = theme;
        _appSettingsService.SaveTheme(theme);
    }

    public void LoadSavedTheme()
    {
        var savedTheme = _appSettingsService.GetTheme();
        ApplyTheme(savedTheme);
    }
}