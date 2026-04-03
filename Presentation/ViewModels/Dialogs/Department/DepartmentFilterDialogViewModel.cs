using System.Windows.Input;
using DocumentFlowing.Common;
using DocumentFlowing.Presentation.ViewModels.Base;
using Documentor.Core.Models.Department;

namespace Documentor.Presentation.ViewModels.Dialogs.Department;

public class DepartmentFilterDialogViewModel : ViewModelBase
{
    private string _title = string.Empty;
    private int _pageSize = 10;
    private string _errorMessage = string.Empty;
    private Action<bool>? _closeAction;

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public int PageSize
    {
        get => _pageSize;
        set => SetProperty(ref _pageSize, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public DepartmentFilterModel ResultFilter { get; private set; } = new();

    public ICommand ApplyCommand { get; }
    public ICommand ResetCommand { get; }
    public ICommand CancelCommand { get; }

    public DepartmentFilterDialogViewModel(DepartmentFilterModel filter)
    {
        _title = filter.Title;
        _pageSize = filter.PageSize == 0 ? 10 : filter.PageSize;

        ApplyCommand = new RelayCommand(Apply);
        ResetCommand = new RelayCommand(Reset);
        CancelCommand = new RelayCommand(() => _closeAction?.Invoke(false));
    }

    public void SetCloseAction(Action<bool> closeAction)
    {
        _closeAction = closeAction;
    }

    private void Apply()
    {
        ResultFilter = new DepartmentFilterModel
        {
            Title = Title,
            PageSize = PageSize <= 0 ? 10 : PageSize,
            PageNumber = 1
        };

        _closeAction?.Invoke(true);
    }

    private void Reset()
    {
        Title = string.Empty;
        PageSize = 10;
    }
}