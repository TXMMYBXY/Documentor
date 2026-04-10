using System.Net.Http;
using DocumentFlowing.Client.Authorization.Dtos;
using DocumentFlowing.Client.Models;
using Documentor.Application.Api.Authorization;
using Documentor.Application.Api.Authorization.Dtos;
using Documentor.Application.Api.Models;
using Microsoft.Extensions.Options;

namespace Documentor.Infrastructure.Api;

public class AuthorizationClient : GeneralClient, IAuthorizationClient
{
    
    public AuthorizationClient(HttpClient httpClient, IOptions<DocumentFlowApi> documentFlowApi) : base(httpClient, documentFlowApi)
    {
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        return await PostResponseAsync<LoginRequestDto, LoginResponseDto>(request, "authorization/login");
    }

    public async Task<RefreshTokenToLoginResponseDto> RequestForAccessAsync(RefreshTokenToLoginRequestDto request)
    {
        return await PostResponseAsync<RefreshTokenToLoginRequestDto, RefreshTokenToLoginResponseDto>(request, "authorization/request-for-access");
    }

    public async Task<AccessTokenResponseDto> GetNewAccessTokenAsync(AccessTokenRequestDto requestDto)
    {
        return await PostResponseAsync<AccessTokenRequestDto, AccessTokenResponseDto>(requestDto, "authorization/access");
    }

    public async Task<RefreshTokenResponseDto> GetNewRefreshTokenAsync(RefreshTokenRequestDto requestDto)
    {
        return await PostResponseAsync<RefreshTokenRequestDto, RefreshTokenResponseDto>(requestDto, "authorization/refresh");
    }
}