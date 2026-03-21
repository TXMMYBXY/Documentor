using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Documentor.Application.Services;

namespace Documentor.Core.Middleware;

public class AuthorizationHandler : DelegatingHandler
{
    private static readonly SemaphoreSlim RefreshLock = new(1, 1);

    private readonly ITokenService _tokenService;

    public AuthorizationHandler(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        await AddAuthorizationHeaderAsync(request);

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode != HttpStatusCode.Unauthorized)
            return response;

        response.Dispose();

        await RefreshLock.WaitAsync(cancellationToken);
        try
        {
            var currentToken = _tokenService.ReturnAccessToken();

            if (string.IsNullOrWhiteSpace(currentToken) || !_tokenService.IsAccessTokenValid())
            {
                var refreshToken = _tokenService.ReturnRefreshToken();
                if (string.IsNullOrWhiteSpace(refreshToken))
                    throw new UnauthorizedAccessException("Refresh token отсутствует.");

                await _tokenService.GetNewAccessTokenAsync();

                if (_tokenService.IsRefreshTokenExpires())
                {
                    await _tokenService.GetNewRefreshTokenAsync();
                }
            }
        }
        finally
        {
            RefreshLock.Release();
        }

        var retryRequest = await CloneRequestAsync(request);
        await AddAuthorizationHeaderAsync(retryRequest);

        return await base.SendAsync(retryRequest, cancellationToken);
    }

    private Task AddAuthorizationHeaderAsync(HttpRequestMessage request)
    {
        var token = _tokenService.ReturnAccessToken();

        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return Task.CompletedTask;
    }

    private static async Task<HttpRequestMessage> CloneRequestAsync(HttpRequestMessage original)
    {
        var clone = new HttpRequestMessage(original.Method, original.RequestUri)
        {
            Version = original.Version,
            VersionPolicy = original.VersionPolicy
        };

        foreach (var header in original.Headers)
        {
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        foreach (var option in original.Options)
        {
            clone.Options.Set(new HttpRequestOptionsKey<object?>(option.Key), option.Value);
        }

        if (original.Content != null)
        {
            var memoryStream = new MemoryStream();
            await original.Content.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            clone.Content = new StreamContent(memoryStream);

            foreach (var header in original.Content.Headers)
            {
                clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        return clone;
    }
}