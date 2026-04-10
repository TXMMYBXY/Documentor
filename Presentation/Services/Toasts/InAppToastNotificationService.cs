using System.Collections.ObjectModel;
using System.Windows;
using Documentor.Core.Interfaces;

namespace Documentor.Presentation.Services.Toasts;

public class InAppToastNotificationService : IToastNotificationService
{
    private readonly ObservableCollection<ToastItemViewModel> _items = new();
    public ReadOnlyObservableCollection<ToastItemViewModel> Notifications { get; }

    public InAppToastNotificationService()
    {
        Notifications = new ReadOnlyObservableCollection<ToastItemViewModel>(_items);
    }

    public void ShowInfo(string title, string message) =>
        Enqueue(title, message, isError: false);

    public void ShowError(string title, string message) =>
        Enqueue(title, message, isError: true);

    private void Enqueue(string title, string message, bool isError)
    {
        _ = System.Windows.Application.Current.Dispatcher.InvokeAsync(async () =>
        {
            var item = new ToastItemViewModel(title, message, isError);

            // показываем внизу (последним)
            _items.Add(item);

            // живёт 4 сек
            await Task.Delay(TimeSpan.FromSeconds(4));

            // запускаем fade-out (анимацию сделаем в XAML по IsClosing)
            item.IsClosing = true;

            // ждём анимацию
            await Task.Delay(TimeSpan.FromMilliseconds(250));

            _items.Remove(item);
        });
    }
}