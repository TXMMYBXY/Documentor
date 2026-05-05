using System.Threading;
using System.Windows.Input;
using Documentor.Core.Enums;
using Documentor.Presentation.ViewModels.Base;

namespace Documentor.Presentation.Services.Toasts;

public sealed class ToastItemViewModel : ViewModelBase
{
    public string Title { get; }
    public string Message { get; }
    public NotificationSeverity Severity { get; }

    // Action button
    private bool _isActionBusy;
    public bool IsActionBusy
    {
        get => _isActionBusy;
        set => SetProperty(ref _isActionBusy, value);
    }

    public string? ActionText { get; }
    public ICommand? ActionCommand { get; }
    public bool HasAction => ActionCommand != null;

    public ICommand CloseCommand { get; }

    private bool _isClosing;
    public bool IsClosing
    {
        get => _isClosing;
        set => SetProperty(ref _isClosing, value);
    }

    internal CancellationTokenSource LifetimeCts { get; } = new();
    internal int ClosingFlag;

    public ToastItemViewModel(
        string title,
        string message,
        NotificationSeverity severity,
        ICommand closeCommand,
        string? actionText = null,
        ICommand? actionCommand = null)
    {
        Title = title;
        Message = message;
        Severity = severity;

        CloseCommand = closeCommand;

        ActionText = actionText;
        ActionCommand = actionCommand;
    }
}