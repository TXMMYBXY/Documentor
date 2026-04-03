namespace Documentor.Core.Interfaces;

public interface IApiEndpointProvider
{
    string GetBaseUrl();
    bool HasOverride();
    string? GetOverrideUrl();
    void SaveOverrideUrl(string url);
    void ClearOverrideUrl();
}