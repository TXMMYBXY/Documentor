using Documentor.Application.Api.Models;
using Documentor.Core.Interfaces;
using Microsoft.Extensions.Options;

namespace Documentor.Infrastructure.Services;

public class ApiEndpointProvider : IApiEndpointProvider
{
    private readonly IOptions<DocumentFlowApi> _options;
    private readonly IAppSettingsService _appSettingsService;

    public ApiEndpointProvider(
        IOptions<DocumentFlowApi> options,
        IAppSettingsService appSettingsService)
    {
        _options = options;
        _appSettingsService = appSettingsService;
    }

    public string GetBaseUrl()
    {
        var overrideUrl = _appSettingsService.GetApiDomainOverride();

        if (!string.IsNullOrWhiteSpace(overrideUrl))
            return NormalizeUrl(overrideUrl);

        return NormalizeUrl(_options.Value.Domain);
    }

    public bool HasOverride()
    {
        return !string.IsNullOrWhiteSpace(_appSettingsService.GetApiDomainOverride());
    }

    public string? GetOverrideUrl()
    {
        return _appSettingsService.GetApiDomainOverride();
    }

    public void SaveOverrideUrl(string url)
    {
        _appSettingsService.SaveApiDomainOverride(NormalizeUrl(url));
    }

    public void ClearOverrideUrl()
    {
        _appSettingsService.ClearApiDomainOverride();
    }

    private static string NormalizeUrl(string url)
    {
        url = url.Trim();

        if (!url.EndsWith("/"))
            url += "/";

        return url;
    }
}