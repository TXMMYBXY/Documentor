using System.Threading;
using Documentor.Core.Enums;
using Documentor.Presentation.ViewModels.Base;

namespace Documentor.Presentation.Services.Toasts;

public sealed class ToastItemViewModel : ViewModelBase
{
    public string Title { get; }
    public string Message { get; }
    public NotificationSeverity Severity { get; }

    private bool _isClosing;
    public bool IsClosing
    {
        get => _isClosing;
        set => SetProperty(ref _isClosing, value);
    }

    internal CancellationTokenSource LifetimeCts { get; } = new();
    internal int ClosingFlag;

    public ToastItemViewModel(string title, string message, NotificationSeverity severity)
    {
        Title = title;
        Message = message;
        Severity = severity;
    }
}