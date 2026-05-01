using AutoMapper;
using Documentor.Application.Api.Authorization;
using Documentor.Application.Api.Authorization.Dtos;
using Documentor.Application.Api.Authorization.Dtos.Requests;
using Documentor.Application.Api.Authorization.Dtos.Responses;
using Documentor.Application.Api.Models;
using Documentor.Application.Services;
using Documentor.Core.Enums;
using Microsoft.Win32;

namespace Documentor.Infrastructure.Services;

public class TokenService : ITokenService
{
    private const string RegistryPath = @"Software\DocumentFlowing\Tokens";
    
    private readonly IDpapiService _dpapiService;
    private readonly IAuthorizationClient _authorizationClient;
    private readonly IMapper _mapper;

    public TokenService(
        IDpapiService dpapiService, 
        IAuthorizationClient authorizationClient,
        IMapper mapper)
    {
        _dpapiService = dpapiService;
        _authorizationClient = authorizationClient;
        _mapper = mapper;
    }
    
    public void SaveTokens(LoginResponseDto loginResponseDto)
    {
        try
        {
            if (loginResponseDto.Access == null && loginResponseDto.Refresh == null) return;

            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(RegistryPath))
            {
                if (key == null)
                {
                    return;
                }

                if (!string.IsNullOrEmpty(loginResponseDto.Access.AccessToken))
                {
                    var encryptedAccessToken = _dpapiService.Encrypt(loginResponseDto.Access.AccessToken);
                    
                    key.SetValue("AccessToken", encryptedAccessToken);
                    key.SetValue("AccessTokenExpires", loginResponseDto.Access.ExpiresAt);
                }

                if (loginResponseDto.Refresh.RefreshToken != null && !string.IsNullOrEmpty(loginResponseDto.Refresh.RefreshToken))
                {
                    var encryptedRefreshToken = _dpapiService.Encrypt(loginResponseDto.Refresh.RefreshToken);
                    
                    key.SetValue("RefreshToken", encryptedRefreshToken);
                    key.SetValue("RefreshTokenExpires", loginResponseDto.Refresh.ExpiresAt);
                }

                if (loginResponseDto.UserInfo != null)
                {
                    key.SetValue("UserEmail", loginResponseDto.UserInfo.Email);
                    key.SetValue("RoleId", loginResponseDto.UserInfo.Role.Id);
                    key.SetValue("Department", loginResponseDto.UserInfo.Department);

                    if (loginResponseDto.Refresh.RefreshToken != null)
                    {
                        key.SetValue("UserId", loginResponseDto.UserInfo.Id);
                    }
                }

                key.SetValue("TokenType", loginResponseDto.Access.TokenType);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving tokens: {ex.Message}");
        }
    }

    public void SaveAccessToken(AccessTokenDto accessTokenDto)
    {
        throw new NotImplementedException();
    }

    public void SaveRefreshToken(RefreshTokenDto refreshTokenResponse)
    {
        try
        {
            if (refreshTokenResponse == null) return;

            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(RegistryPath))
            {
                if (key == null) return;
                
                if (refreshTokenResponse.RefreshToken != null && !string.IsNullOrEmpty(refreshTokenResponse.RefreshToken))
                {
                    var encryptedRefreshToken = _dpapiService.Encrypt(refreshTokenResponse.RefreshToken);
                    key.SetValue("RefreshToken", encryptedRefreshToken);
                    key.SetValue("RefreshTokenExpires", refreshTokenResponse.ExpiresAt);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving token: {ex.Message}");
        }
    }

    public string ReturnAccessToken()
    {
        try
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryPath))
            {
                if (key == null)
                {
                    return null;
                }

                var encryptedToken = key.GetValue("AccessToken") as string;
                
                if (string.IsNullOrEmpty(encryptedToken)) return null;

                return _dpapiService.Decrypt(encryptedToken);
            }
        }
        catch
        {
            return null;
        }
    }

    public string ReturnRefreshToken()
    {
        try
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryPath))
            {
                if (key == null)
                {
                    return null;
                }

                var encryptedToken = key.GetValue("RefreshToken") as string;
                if (string.IsNullOrEmpty(encryptedToken)) return null;

                return _dpapiService.Decrypt(encryptedToken);
            }
        }
        catch
        {
            return null;
        }
    }

    public UserInfoDto GetUserInfo()
    {
        try
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryPath))
            {
                if (key == null)
                {
                    return null;
                }

                var roleId = key.GetValue("RoleId") as int?;
                if (!roleId.HasValue) return null;
                Enum.TryParse<UserRole>(roleId.ToString(), out var role);
                
                return new UserInfoDto
                {
                    Email = key.GetValue("UserEmail") as string,
                    Role = new Role
                    {
                        Id = roleId.Value,
                        Title = role.ToString()
                    },
                    Department = key.GetValue("Department") as string
                };
            }
        }
        catch
        {
            return null;
        }
    }

    public async Task<string> GetNewAccessTokenAsync()
    {
        var request = new AccessTokenRequestDto
        {
            RefreshToken = ReturnRefreshToken(),
        };

        if (string.IsNullOrEmpty(request.RefreshToken))
        {
            throw new NullReferenceException("Refresh token is out");
        }
        
        var token = await _authorizationClient.GetNewAccessTokenAsync(request);

        if (token == null)
        {
            throw new NullReferenceException("Access token is not got");
        }

        var mapedToken = new LoginResponseDto
        {
            Access = _mapper.Map<AccessTokenDto>(token)
        };
        
        SaveTokens(mapedToken);

        return token.AccessToken;
    }

    public async Task GetNewRefreshTokenAsync()
    {
        var request = new RefreshTokenRequestDto
        {
            Token = ReturnRefreshToken()
        };

        if (string.IsNullOrEmpty(request.Token))
        {
            throw new NullReferenceException("Refresh token is out");
        }
        
        var token = await _authorizationClient.GetNewRefreshTokenAsync(request);
        
        SaveRefreshToken(_mapper.Map<RefreshTokenDto>(token));
    }

    public bool IsRefreshTokenExpires()
    {
        try
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryPath))
            {
                if (key == null)
                {
                    return false;
                }

                var expiresAtStr = key.GetValue("RefreshTokenExpires") as string;
                
                if (!string.IsNullOrEmpty(expiresAtStr) &&
                    DateTime.TryParse(expiresAtStr, out DateTime expiresAt))
                {
                    return expiresAt - DateTime.UtcNow < TimeSpan.FromDays(1);
                }

                return false;
            }
        }
        catch
        {
            return false;
        }
    }

    public void ClearTokens()
    {
        try
        {
            Registry.CurrentUser.DeleteSubKeyTree(RegistryPath, false);
        }
        catch
        {
            // Ошибки игнорируются, если ключ не существует
        }
    }

    public bool IsAccessTokenValid()
    {
        try
        {
            var token = ReturnAccessToken();
            if (string.IsNullOrEmpty(token)) return false;

            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryPath))
            {
                if (key == null) return false;

                // Проверяем срок действия токена
                var expiresAtStr = key.GetValue("AccessTokenExpires") as string;
                if (!string.IsNullOrEmpty(expiresAtStr))
                {
                    // Парсим дату в формате "dd.MM.yyyy HH:mm:ss"
                    if (DateTime.TryParseExact(expiresAtStr, "dd.MM.yyyy HH:mm:ss",
                            System.Globalization.CultureInfo.InvariantCulture,
                            System.Globalization.DateTimeStyles.None, out DateTime expiresAt))
                    {
                        return expiresAt > DateTime.Now;
                    }
                    else
                    {
                        if (DateTime.TryParse(expiresAtStr, out expiresAt))
                        {
                            return expiresAt > DateTime.Now.AddMinutes(5);
                        }
                    }
                }

                return false;
            }
        }
        catch
        {
            return false;
        }
    }
    
    public bool IsRefreshTokenValid()
    {
        try
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryPath))
            {
                if (key == null) return false;

                var expiresAtStr = key.GetValue("RefreshTokenExpires") as string;
                if (!string.IsNullOrEmpty(expiresAtStr) &&
                    DateTime.TryParse(expiresAtStr, out DateTime expiresAt))
                {
                    return expiresAt > DateTime.UtcNow;
                }

                return false;
            }
        }
        catch
        {
            return false;
        }
    }
}