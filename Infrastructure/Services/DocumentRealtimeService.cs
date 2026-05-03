using System.Diagnostics;
using Documentor.Application.Services;
using Documentor.Core.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;

namespace Documentor.Infrastructure.Services;

public class DocumentRealtimeService : IDocumentRealtimeService
{
    private readonly IApiEndpointProvider _apiEndpointProvider;
    private readonly ITokenService _tokenService;

    private HubConnection? _connection;

    public event Action<int>? DocumentReadyReceived;

    public DocumentRealtimeService(IApiEndpointProvider apiEndpointProvider, ITokenService tokenService)
    {
        _apiEndpointProvider = apiEndpointProvider;
        _tokenService = tokenService;
    }

    public async Task StartAsync()
    {
        if (_connection != null)
            return;

        var hubUrl = BuildDocumentsHubUrl(_apiEndpointProvider.GetBaseUrl());
        Debug.WriteLine($"[DocumentRealtimeService] HubUrl: {hubUrl}");

        _connection = new HubConnectionBuilder()
            .WithUrl(hubUrl, options =>
            {
                options.AccessTokenProvider = () =>
                {
                    var token = _tokenService.AccessToken.AccessToken;
                    return Task.FromResult(token);
                };
            })
            .WithAutomaticReconnect()
            .Build();

        _connection.On<int>("downloadDocument", documentId =>
        {
            Debug.WriteLine($"[DocumentRealtimeService] downloadDocument: {documentId}");
            DocumentReadyReceived?.Invoke(documentId);
        });

        await _connection.StartAsync();
        Debug.WriteLine($"[DocumentRealtimeService] Connected. ConnectionId={_connection.ConnectionId}");
    }

    public async Task StopAsync()
    {
        if (_connection == null)
            return;

        await _connection.StopAsync();
        await _connection.DisposeAsync();
        _connection = null;
    }

    private static string BuildDocumentsHubUrl(string apiBaseUrl)
    {
        var url = apiBaseUrl.TrimEnd('/');

        if (url.EndsWith("/api", StringComparison.OrdinalIgnoreCase))
            url = url[..^4];

        return $"{url}/documents";
    }
}