using System.Windows.Input;
using DocumentFlowing.Common;
using DocumentFlowing.Presentation.ViewModels.Base;
using Documentor.Core.Interfaces;
using Documentor.Core.Models.Department;

namespace Documentor.Presentation.ViewModels.Dialogs.Department;

public class EditDepartmentDialogViewModel : ViewModelBase
{
    private readonly IDepartmentManagementService _departmentManagementService;
    private readonly int _departmentId;

    private string _title = string.Empty;
    private string _description = string.Empty;
    private string _errorMessage = string.Empty;
    private Action<bool>? _closeAction;

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    public EditDepartmentDialogViewModel(
        IDepartmentManagementService departmentManagementService,
        int departmentId,
        DepartmentListItemModel department)
    {
        _departmentManagementService = departmentManagementService;
        _departmentId = departmentId;

        _title = department.Title;
        _description = department.Description;

        SaveCommand = new AsyncRelayCommand(SaveAsync);
        CancelCommand = new RelayCommand(() => _closeAction?.Invoke(false));
    }

    public void SetCloseAction(Action<bool> closeAction)
    {
        _closeAction = closeAction;
    }

    private async Task SaveAsync()
    {
        try
        {
            ErrorMessage = string.Empty;

            await _departmentManagementService.UpdateDepartmentAsync(_departmentId, new EditDepartmentModel
            {
                Title = Title,
                Description = Description
            });

            _closeAction?.Invoke(true);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка обновления отдела: {ex.Message}";
        }
    }
}