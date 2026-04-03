using Documentor.Core.Enums;

namespace Documentor.Core.Interfaces;

public interface IAppSettingsService
{
    AppTheme GetTheme();
    void SaveTheme(AppTheme theme);

    int GetPageSize();
    void SavePageSize(int pageSize);
}