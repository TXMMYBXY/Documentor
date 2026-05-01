using System.Windows;
using Documentor.Application.Api.Authorization;
using Documentor.Application.Api.Authorization.Dtos;
using Documentor.Application.Api.Authorization.Dtos.Requests;
using Documentor.Application.Services;

namespace Documentor.Infrastructure.Services;

public class AuthorizationService :  IAuthorizationService
{
    private readonly ITokenService _tokenService;
    private readonly IAuthorizationClient _authorizationClient;

    public AuthorizationService(
        ITokenService tokenService, 
        IAuthorizationClient authorizationClient)
    {
        _tokenService = tokenService;
        _authorizationClient = authorizationClient;
    }
    
    public async Task<bool> TryAutoLoginAsync()
    {
        try
        {
            var request = new RefreshTokenToLoginRequestDto
            {
                RefreshToken = _tokenService.ReturnRefreshToken()
            };
            
            if (!string.IsNullOrEmpty(request.RefreshToken))
            {
                var refreshToken 
                    = await _authorizationClient.RequestForAccessAsync(request);

                if (refreshToken.IsAllowed)
                {
                    return true;
                }
            }
            
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Auto-login failed: {ex.Message}");
            return false;
        }
    }

    public async Task<int?> LoginAsync(string email, string password)
    {
        try
        {
            var loginRequest = new LoginRequestDto
            {
                Email = email,
                Password = password
            };

            var response = await _authorizationClient.LoginAsync(loginRequest);

            if (response != null && !string.IsNullOrEmpty(response.Access.AccessToken))
            {
                _tokenService.SaveTokens(response);

                return _tokenService.GetUserInfo().Role.Id;
            }

            return null;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Auth failed: {ex.Message}", "Ошибка",
            MessageBoxButton.OK, MessageBoxImage.Error);
            
            return null;
        }
    }
}
