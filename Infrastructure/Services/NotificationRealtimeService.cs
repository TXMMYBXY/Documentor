using System.Diagnostics;
using Documentor.Application.Services;
using Documentor.Core.Interfaces;
using Documentor.Core.Models;
using Microsoft.AspNetCore.SignalR.Client;

namespace Documentor.Infrastructure.Services;

public class NotificationRealtimeService : INotificationRealtimeService
{
    private readonly IApiEndpointProvider _apiEndpointProvider;
    private readonly ITokenService _tokenService;

    private HubConnection? _connection;

    public event Action<RealtimeNotification>? NotificationReceived;

    public NotificationRealtimeService(
        IApiEndpointProvider apiEndpointProvider,
        ITokenService tokenService)
    {
        _apiEndpointProvider = apiEndpointProvider;
        _tokenService = tokenService;
    }

    public async Task StartAsync()
    {
        if (_connection != null)
            return;

        var hubUrl = BuildNotificationsHubUrl(_apiEndpointProvider.GetBaseUrl());
        Debug.WriteLine($"[NotificationRealtimeService] HubUrl: {hubUrl}");

        _connection = new HubConnectionBuilder()
            .WithUrl(hubUrl, options =>
            {
                options.AccessTokenProvider = () =>
                {
                    var token = _tokenService.ReturnAccessToken();
                    
                    return Task.FromResult(token);
                };
            })
            .WithAutomaticReconnect()
            .Build();

        _connection.On<RealtimeNotification>("Notification", message =>
        {
            Debug.WriteLine($"[NotificationRealtimeService] NewTemplate: {message}");
            NotificationReceived?.Invoke(message);
        });

        await _connection.StartAsync();
        Debug.WriteLine($"[NotificationRealtimeService] Connected. ConnectionId={_connection.ConnectionId}");
    }

    public async Task StopAsync()
    {
        if (_connection == null)
            return;

        await _connection.StopAsync();
        await _connection.DisposeAsync();
        _connection = null;
    }

    private static string BuildNotificationsHubUrl(string apiBaseUrl)
    {
        var url = apiBaseUrl.TrimEnd('/');

        if (url.EndsWith("/api", StringComparison.OrdinalIgnoreCase))
            url = url[..^4];

        return $"{url}/notifications";
    }
}