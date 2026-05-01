namespace Documentor.Core.Interfaces;

public interface IToastNotificationService
{
    void ShowInfo(string title, string message);
    void ShowSuccess(string title, string message);
    void ShowError(string title, string message);
    
}