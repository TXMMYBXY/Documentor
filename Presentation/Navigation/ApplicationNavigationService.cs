using Documentor.Application.Services;
using Documentor.Core.Interfaces;
using Documentor.Presentation.Views.Windows;

namespace Documentor.Presentation.Navigation;

public class ApplicationNavigationService : IApplicationNavigationService
{
    private readonly IWindowService _windowService;
    private readonly IUserSession _userSession;
    private readonly ITokenService _tokenService;

    public ApplicationNavigationService(
        IWindowService windowService,
        IUserSession userSession,
        ITokenService tokenService)
    {
        _windowService = windowService;
        _userSession = userSession;
        _tokenService = tokenService;
    }

    public void ShowLogin()
    {
        _windowService.ReplaceMainWindow<LoginWindow>();
    }

    public void ShowMainShell()
    {
        _windowService.ReplaceMainWindow<MainShellWindow>();
    }

    public void Logout()
    {
        _userSession.Clear();
        _tokenService.ClearTokens();
        ShowLogin();
    }
}