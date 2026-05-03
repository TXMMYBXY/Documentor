namespace Documentor.Core.Interfaces;

public interface IDocumentRealtimeService
{
    event Action<int>? DocumentReadyReceived;
    Task StartAsync();
    Task StopAsync();
}