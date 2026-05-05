namespace Documentor.Core.Interfaces;

public interface INotificationCoordinator
{
    Task StartAsync();
    Task StopAsync();
}