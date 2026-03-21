using AutoMapper;
using Documentor.Application.Services;
using Documentor.Core.Interfaces;
using Documentor.Core.Models;
using Documentor.Presentation.Navigation;

namespace Documentor.Infrastructure.Services;

public class AppStartupService : IAppStartupService
{
    private readonly IAuthorizationService _authorizationService;
    private readonly ITokenService _tokenService;
    private readonly IUserSession _userSession;
    private readonly IApplicationNavigationService _applicationNavigationService;
    private readonly IMapper _mapper;

    public AppStartupService(
        IAuthorizationService authorizationService,
        ITokenService tokenService,
        IUserSession userSession,
        IApplicationNavigationService applicationNavigationService,
        IMapper mapper)
    {
        _authorizationService = authorizationService;
        _tokenService = tokenService;
        _userSession = userSession;
        _applicationNavigationService = applicationNavigationService;
        _mapper = mapper;
    }

    public async Task StartAsync()
    {
        var success = await _authorizationService.TryAutoLoginAsync();

        if (!success)
        {
            _applicationNavigationService.ShowLogin();
            return;
        }

        var userInfoDto = _tokenService.GetUserInfo();
        if (userInfoDto == null)
        {
            _applicationNavigationService.ShowLogin();
            return;
        }

        var userInfo = _mapper.Map<UserInfoModel>(userInfoDto);
        _userSession.Set(userInfo);

        _applicationNavigationService.ShowMainShell();
    }
}