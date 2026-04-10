using Documentor.Core.Enums;
using Documentor.Core.Interfaces;
using Documentor.Core.Models;

namespace Documentor.Presentation.Services;

public class NotificationCoordinator : INotificationCoordinator
{
    private readonly INotificationRealtimeService _realtime;
    private readonly IToastNotificationService _toast;

    private bool _started;

    public NotificationCoordinator(INotificationRealtimeService realtime, IToastNotificationService toast)
    {
        _realtime = realtime;
        _toast = toast;
    }

    public async Task StartAsync()
    {
        if (_started)
            return;

        _started = true;

        _realtime.NotificationReceived += _OnNewMessage;
        await _realtime.StartAsync();
    }

    public async Task StopAsync()
    {
        if (!_started)
            return;

        _started = false;

        _realtime.NotificationReceived -= _OnNewMessage;
        await _realtime.StopAsync();
    }

    private void _OnNewMessage(RealtimeNotification realtimeNotification)
    {
        switch (realtimeNotification.Severity)
        {
            case NotificationSeverity.Error:
                _toast.ShowError(realtimeNotification.Title, realtimeNotification.Message);
                break;

            default:
                _toast.ShowInfo(realtimeNotification.Title, realtimeNotification.Message);
                break;
        }
    }
}