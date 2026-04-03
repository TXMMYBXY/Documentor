using Documentor.Core.Enums;
using Documentor.Core.Interfaces;
using Documentor.Presentation.ViewModels.Base;

namespace Documentor.Presentation.ViewModels.Pages
{
    public class SettingsPageViewModel : ViewModelBase
    {
        private readonly IThemeService _themeService;
        private readonly IAppSettingsService _appSettingsService;

        private bool _isDarkTheme;
        private int _selectedPageSize;

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

        public int MinPageSize => 5;
        public int MaxPageSize => 100;
        public int PageSizeTickFrequency => 5;

        public int SelectedPageSize
        {
            get => _selectedPageSize;
            set
            {
                var normalized = NormalizePageSize(value);

                if (SetProperty(ref _selectedPageSize, normalized))
                {
                    _appSettingsService.SavePageSize(normalized);
                }
            }
        }

        public SettingsPageViewModel(
            IThemeService themeService,
            IAppSettingsService appSettingsService)
        {
            _themeService = themeService;
            _appSettingsService = appSettingsService;

            _isDarkTheme = _themeService.CurrentTheme == AppTheme.Dark;
            _selectedPageSize = NormalizePageSize(_appSettingsService.GetPageSize());
        }

        private int NormalizePageSize(int value)
        {
            if (value < MinPageSize)
                value = MinPageSize;

            if (value > MaxPageSize)
                value = MaxPageSize;

            var remainder = value % PageSizeTickFrequency;
            if (remainder != 0)
            {
                value -= remainder;
                if (value < MinPageSize)
                    value = MinPageSize;
            }

            return value;
        }
    }
}