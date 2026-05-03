using Documentor.Core.Enums;

namespace Documentor.Core.Interfaces;

public interface IToastNotificationService
{
    void ShowInfo(string title, string message);
    void ShowSuccess(string title, string message);
    void ShowError(string title, string message);
    void ShowAction(string title, string message, NotificationSeverity severity, 
        string actionText, Func<Task> action, TimeSpan? duration = null);
}