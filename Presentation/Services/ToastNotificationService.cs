using System.Collections.ObjectModel;
using Documentor.Core.Enums;
using Documentor.Core.Interfaces;
using Documentor.Presentation.Services.Toasts;

namespace Documentor.Presentation.Services;

public class ToastNotificationService : IToastNotificationService, IInAppToastSource
{
    private const int MaxVisible = 3;

    // Длительность показа и длительность fade-out должны совпадать с XAML
    private static readonly TimeSpan DefaultDuration = TimeSpan.FromSeconds(4);
    private static readonly TimeSpan FadeOutDuration = TimeSpan.FromMilliseconds(260);

    private readonly ObservableCollection<ToastItemViewModel> _notifications = new();
    public ReadOnlyObservableCollection<ToastItemViewModel> Notifications { get; }

    public ToastNotificationService()
    {
        Notifications = new ReadOnlyObservableCollection<ToastItemViewModel>(_notifications);
    }

    public void ShowInfo(string title, string message) => 
        _ = ShowAsync(title, message, NotificationSeverity.Info);
    public void ShowSuccess(string title, string message) => 
        _ = ShowAsync(title, message, NotificationSeverity.Success);
    public void ShowError(string title, string message) => 
    _ = ShowAsync(title, message, NotificationSeverity.Error);

    private async Task ShowAsync(string title, string message, NotificationSeverity severity)
    {
        await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
        {
            // если уже 3 — закрываем самый старый (верхний)
            if (_notifications.Count >= MaxVisible)
                _ = CloseAndRemoveAsync(_notifications[0]);

            var item = new ToastItemViewModel(title, message, severity);
            _notifications.Add(item);

            _ = AutoCloseAsync(item, DefaultDuration);
        });
    }

    private async Task AutoCloseAsync(ToastItemViewModel item, TimeSpan duration)
    {
        try
        {
            await Task.Delay(duration, item.LifetimeCts.Token);
        }
        catch (TaskCanceledException)
        {
            // закрыли раньше времени — ок
        }

        await CloseAndRemoveAsync(item);
    }

    private async Task CloseAndRemoveAsync(ToastItemViewModel item)
    {
        if (Interlocked.Exchange(ref item.ClosingFlag, 1) == 1)
            return;

        item.LifetimeCts.Cancel();

        await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
        {
            // триггерит fade-out в XAML
            item.IsClosing = true;
        });

        // дождаться fade-out
        await Task.Delay(FadeOutDuration);

        await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
        {
            _notifications.Remove(item);
        });
    }
}