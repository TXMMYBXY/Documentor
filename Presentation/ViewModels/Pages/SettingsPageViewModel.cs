using System.Windows.Input;
using DocumentFlowing.Common;
using DocumentFlowing.Presentation.ViewModels.Base;
using Documentor.Core.Enums;
using Documentor.Core.Interfaces;

namespace Documentor.Presentation.ViewModels.Pages;

public class SettingsPageViewModel : ViewModelBase
{
    private readonly IThemeService _themeService;

    public AppTheme SelectedTheme
    {
        get => _themeService.CurrentTheme;
        set
        {
            _themeService.ApplyTheme(value);
            OnPropertyChanged();
        }
    }

    public ICommand SetLightThemeCommand { get; }
    public ICommand SetDarkThemeCommand { get; }

    public SettingsPageViewModel(IThemeService themeService)
    {
        _themeService = themeService;

        SetLightThemeCommand = new RelayCommand(() =>
            _themeService.ApplyTheme(AppTheme.Light));

        SetDarkThemeCommand = new RelayCommand(() =>
            _themeService.ApplyTheme(AppTheme.Dark));
    }
}