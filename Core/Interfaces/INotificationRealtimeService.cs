using Documentor.Core.Models;

namespace Documentor.Core.Interfaces;

public interface INotificationRealtimeService
{
    event Action<RealtimeNotification>? NotificationReceived;

    Task StartAsync();
    Task StopAsync();
}