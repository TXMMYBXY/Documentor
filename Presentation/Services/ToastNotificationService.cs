using System.Collections.ObjectModel;
using Documentor.Common;
using Documentor.Core.Enums;
using Documentor.Core.Interfaces;
using Documentor.Presentation.Services.Toasts;

namespace Documentor.Presentation.Services;

public class ToastNotificationService : IToastNotificationService, IInAppToastSource
{
    private const int MaxVisible = 3;

    private static readonly TimeSpan DefaultDuration = TimeSpan.FromSeconds(4);
    private static readonly TimeSpan ActionDuration = TimeSpan.FromMinutes(1);
    private static readonly TimeSpan FadeOutDuration = TimeSpan.FromMilliseconds(260);

    private readonly ObservableCollection<ToastItemViewModel> _notifications = new();
    public ReadOnlyObservableCollection<ToastItemViewModel> Notifications { get; }

    public ToastNotificationService()
    {
        Notifications = new ReadOnlyObservableCollection<ToastItemViewModel>(_notifications);
    }

    public void ShowInfo(string title, string message) =>
        _ = ShowAsync(title, message, NotificationSeverity.Info, DefaultDuration);

    public void ShowSuccess(string title, string message) =>
        _ = ShowAsync(title, message, NotificationSeverity.Success, DefaultDuration);

    public void ShowError(string title, string message) =>
        _ = ShowAsync(title, message, NotificationSeverity.Error, DefaultDuration);

    public void ShowAction(string title, string message, NotificationSeverity severity, string actionText, Func<Task> action, TimeSpan? duration = null)
        => _ = ShowActionAsync(title, message, severity, actionText, action, duration ?? ActionDuration);

    private async Task ShowAsync(string title, string message, NotificationSeverity severity, TimeSpan duration)
    {
        await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
        {
            if (_notifications.Count >= MaxVisible)
                _ = CloseAndRemoveAsync(_notifications[0]);

            var closeCmd = new RelayCommand(() => _ = CloseAndRemoveAsync(item: null)); 
        });

        await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
        {
            if (_notifications.Count >= MaxVisible)
                _ = CloseAndRemoveAsync(_notifications[0]);

            ToastItemViewModel? created = null;

            var closeCommand = new RelayCommand(() => _ = CloseAndRemoveAsync(created!));
            created = new ToastItemViewModel(title, message, severity, closeCommand);

            _notifications.Add(created);
            _ = AutoCloseAsync(created, duration);
        });
    }

    private async Task ShowActionAsync(string title, string message, NotificationSeverity severity, string actionText, Func<Task> action, TimeSpan duration)
    {
        await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
        {
            if (_notifications.Count >= MaxVisible)
                _ = CloseAndRemoveAsync(_notifications[0]);

            ToastItemViewModel? created = null;

            var closeCommand = new RelayCommand(() => _ = CloseAndRemoveAsync(created!));

            var actionCommand = new AsyncRelayCommand(async () =>
            {
                if (created == null || created.IsActionBusy)
                    return;

                try
                {
                    created.IsActionBusy = true;
                    created.LifetimeCts.Cancel();

                    await action();

                    
                    await CloseAndRemoveAsync(created);
                }
                catch (Exception ex)
                {
                    
                    ShowError("Ошибка", ex.Message);
                    created.IsActionBusy = false;

                    
                }
            });

            created = new ToastItemViewModel(title, message, severity, closeCommand, actionText, actionCommand);

            _notifications.Add(created);

            
            _ = AutoCloseAsync(created, duration);
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
            return;
        }

        await CloseAndRemoveAsync(item);
    }

    private async Task CloseAndRemoveAsync(ToastItemViewModel item)
    {
        if (item == null)
            return;

        if (Interlocked.Exchange(ref item.ClosingFlag, 1) == 1)
            return;

        item.LifetimeCts.Cancel();

        await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
        {
            item.IsClosing = true;
        });

        await Task.Delay(FadeOutDuration);

        await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
        {
            _notifications.Remove(item);
        });
    }
}