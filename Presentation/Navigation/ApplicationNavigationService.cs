using Documentor.Application.Services;
using Documentor.Core.Interfaces;
using Documentor.Presentation.Views.Windows;

namespace Documentor.Presentation.Navigation;

public class ApplicationNavigationService : IApplicationNavigationService
{
    private readonly IWindowService _windowService;
    private readonly IUserSession _userSession;
    private readonly ITokenService _tokenService;
    private readonly INotificationCoordinator _notificationCoordinator;

    public ApplicationNavigationService(
        IWindowService windowService,
        IUserSession userSession,
        ITokenService tokenService,
        INotificationCoordinator notificationCoordinator)
    {
        _windowService = windowService;
        _userSession = userSession;
        _tokenService = tokenService;
        _notificationCoordinator = notificationCoordinator;
    }

    public void ShowLogin()
    {
        _windowService.ReplaceMainWindow<LoginWindow>();
    }

    public void ShowMainShell()
    {
        _windowService.ReplaceMainWindow<MainShellWindow>();
        
        _ = _notificationCoordinator.StartAsync();
    }

    public void Logout()
    {
        _ = _notificationCoordinator.StopAsync();
        _userSession.Clear();
        _tokenService.ClearTokens();
        ShowLogin();
    }
}