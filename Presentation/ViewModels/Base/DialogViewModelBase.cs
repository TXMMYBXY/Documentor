using System.Windows.Input;
using Documentor.Common;

namespace Documentor.Presentation.ViewModels.Base;

public abstract class DialogViewModelBase : ViewModelBase
{
    private string _errorMessage = string.Empty;
    private bool _isBusy;

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    public Action<bool?>? CloseRequested { get; set; }

    public ICommand CancelCommand { get; }

    protected DialogViewModelBase()
    {
        CancelCommand = new RelayCommand(() => RequestClose(false));
    }

    protected void RequestClose(bool? result)
    {
        CloseRequested?.Invoke(result);
    }
}