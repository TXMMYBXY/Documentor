using System.Windows.Input;
using AutoMapper;
using DocumentFlowing.Common;
using DocumentFlowing.Presentation.ViewModels.Base;
using Documentor.Application.Services;
using Documentor.Core.Interfaces;
using Documentor.Core.Models;
using Documentor.Presentation.Navigation;

namespace Documentor.Presentation.ViewModels.Windows;

public class LoginWindowViewModel : ViewModelBase
{
    private readonly IAuthorizationService _authorizationService;
    private readonly ITokenService _tokenService;
    private readonly IUserSession _userSession;
    private readonly IApplicationNavigationService _applicationNavigationService;
    private readonly IMapper _mapper;

    private string _email = string.Empty;
    private string _password = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isBusy;

    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    public ICommand LoginCommand { get; }

    public LoginWindowViewModel(
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

        LoginCommand = new AsyncRelayCommand(LoginAsync);
    }

    private async Task LoginAsync()
    {
        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Введите логин и пароль";
                return;
            }

            var roleId = await _authorizationService.LoginAsync(Email, Password);

            if (roleId == null)
            {
                ErrorMessage = "Неверный логин или пароль";
                return;
            }

            var userInfoDto = _tokenService.GetUserInfo();
            if (userInfoDto == null)
            {
                ErrorMessage = "Не удалось получить данные пользователя";
                return;
            }

            var userInfo = _mapper.Map<UserInfoModel>(userInfoDto);
            _userSession.Set(userInfo);

            _applicationNavigationService.ShowMainShell();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка авторизации: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}