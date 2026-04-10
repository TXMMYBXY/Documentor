using System.Windows.Input;
using DocumentFlowing.Common;
using Documentor.Common;
using Documentor.Presentation.ViewModels.Base;

namespace Documentor.Presentation.ViewModels.Dialogs.Common;

public class ConfirmationDialogViewModel : ViewModelBase
{
    private readonly Action<bool> _closeAction;

    public string Title { get; }
    public string Message { get; }
    public string ConfirmText { get; }
    public string CancelText { get; }

    public ICommand ConfirmCommand { get; }
    public ICommand CancelCommand { get; }

    public ConfirmationDialogViewModel(
        string title,
        string message,
        Action<bool> closeAction,
        string confirmText = "Удалить",
        string cancelText = "Отмена")
    {
        Title = title;
        Message = message;
        ConfirmText = confirmText;
        CancelText = cancelText;
        _closeAction = closeAction;

        ConfirmCommand = new RelayCommand(() => _closeAction(true));
        CancelCommand = new RelayCommand(() => _closeAction(false));
    }
}