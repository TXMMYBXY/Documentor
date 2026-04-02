using System.Windows.Input;
using DocumentFlowing.Common;
using DocumentFlowing.Presentation.ViewModels.Base;
using Documentor.Core.Enums;
using Documentor.Core.Interfaces;

namespace Documentor.Presentation.ViewModels.Pages
{
    public class SettingsPageViewModel : ViewModelBase
    {
        private readonly IThemeService _themeService;

        private bool _isDarkTheme;
        public bool IsDarkTheme
        {
            get => _isDarkTheme;
            set
            {
                if (SetProperty(ref _isDarkTheme, value))
                {
                    _themeService.ApplyTheme(value ? AppTheme.Dark : AppTheme.Light);
                }
            }
        }

        public SettingsPageViewModel(IThemeService themeService)
        {
            _themeService = themeService;
            _isDarkTheme = _themeService.CurrentTheme == AppTheme.Dark;
        }
    }
}