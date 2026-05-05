using System.IO;
using Documentor.Application.Api.Document;
using Microsoft.Win32;
using Documentor.Core.Enums;
using Documentor.Core.Interfaces;
using Documentor.Core.Models;

namespace Documentor.Presentation.Services;

public class NotificationCoordinator : INotificationCoordinator
{
    private readonly INotificationRealtimeService _realtime;
    private readonly IDocumentRealtimeService _documentRealtime;
    private readonly IDocumentClient _documentClient;
    private readonly IToastNotificationService _toast;

    private bool _started;

    public NotificationCoordinator(
        INotificationRealtimeService realtime,
        IDocumentRealtimeService documentRealtime,
        IDocumentClient documentClient,
        IToastNotificationService toast)
    {
        _realtime = realtime;
        _documentRealtime = documentRealtime;
        _documentClient = documentClient;
        _toast = toast;
    }

    public async Task StartAsync()
    {
        if (_started)
            return;

        _started = true;

        _realtime.NotificationReceived += OnNotification;
        _documentRealtime.DocumentReadyReceived += _OnDocumentReady;

        await _realtime.StartAsync();
        await _documentRealtime.StartAsync();
    }

    public async Task StopAsync()
    {
        if (!_started)
            return;

        _started = false;

        _realtime.NotificationReceived -= OnNotification;
        _documentRealtime.DocumentReadyReceived -= _OnDocumentReady;

        await _realtime.StopAsync();
        await _documentRealtime.StopAsync();
    }

    private void OnNotification(RealtimeNotification realtimeNotification)
    {
        switch (realtimeNotification.Severity)
        {
            case NotificationSeverity.Success:
                _toast.ShowSuccess(realtimeNotification.Title, realtimeNotification.Message);
                break;
            case NotificationSeverity.Error:
                _toast.ShowError(realtimeNotification.Title, realtimeNotification.Message);
                break;
            default:
                _toast.ShowInfo(realtimeNotification.Title, realtimeNotification.Message);
                break;
        }
    }

    private void _OnDocumentReady(int documentId)
    {
        _toast.ShowAction(
            title: "Документ готов",
            message: $"Документ №{documentId} готов к скачиванию",
            severity: NotificationSeverity.Success,
            actionText: "Скачать",
            action: async () =>
            {
                await using var file = await _documentClient.DownloadDocumentAsync(documentId);

                var dlg = new SaveFileDialog
                {
                    Filter = "Word Document (*.docx)|*.docx|All files (*.*)|*.*",
                    FileName = file.FileName
                };

                if (dlg.ShowDialog() != true)
                    return;

                await using var outStream = File.Create(dlg.FileName);
                await file.ContentStream.CopyToAsync(outStream);
            },
            duration: TimeSpan.FromMinutes(1));
    }
}