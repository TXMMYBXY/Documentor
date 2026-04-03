using System.Windows.Input;
using AutoMapper;
using DocumentFlowing.Common;
using Documentor.Application.Services;
using Documentor.Core.Interfaces;
using Documentor.Core.Models.User;
using Documentor.Presentation.Navigation;
using Documentor.Presentation.ViewModels.Base;

namespace Documentor.Presentation.ViewModels.Windows;

public class LoginWindowViewModel : ViewModelBase
{
    private readonly IAuthorizationService _authorizationService;
    private readonly ITokenService _tokenService;
    private readonly IUserSession _userSession;
    private readonly IApplicationNavigationService _applicationNavigationService;
    private readonly IMapper _mapper;
    private readonly IApiEndpointProvider _apiEndpointProvider;

    private Action? _openApiSettingsAction;

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
        set
        {
            if (SetProperty(ref _errorMessage, value))
            {
                OnPropertyChanged(nameof(HasError));
            }
        }
    }

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            if (SetProperty(ref _isBusy, value))
            {
                if (LoginCommand is AsyncRelayCommand command)
                    command.RaiseCanExecuteChanged();
            }
        }
    }

    public string CurrentApiUrl => _apiEndpointProvider.GetBaseUrl();

    public ICommand LoginCommand { get; }
    public ICommand OpenApiSettingsCommand { get; }

    public LoginWindowViewModel(
        IAuthorizationService authorizationService,
        ITokenService tokenService,
        IUserSession userSession,
        IApplicationNavigationService applicationNavigationService,
        IMapper mapper,
        IApiEndpointProvider apiEndpointProvider)
    {
        _authorizationService = authorizationService;
        _tokenService = tokenService;
        _userSession = userSession;
        _applicationNavigationService = applicationNavigationService;
        _mapper = mapper;
        _apiEndpointProvider = apiEndpointProvider;

        LoginCommand = new AsyncRelayCommand(LoginAsync, () => !IsBusy);
        OpenApiSettingsCommand = new RelayCommand(OpenApiSettings);
    }

    public void SetOpenApiSettingsAction(Action openApiSettingsAction)
    {
        _openApiSettingsAction = openApiSettingsAction;
    }

    public void RefreshApiUrl()
    {
        OnPropertyChanged(nameof(CurrentApiUrl));
    }

    public IApiEndpointProvider GetApiEndpointProvider()
    {
        return _apiEndpointProvider;
    }

    private void OpenApiSettings()
    {
        _openApiSettingsAction?.Invoke();
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