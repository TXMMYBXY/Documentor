using System.Windows.Input;
using Documentor.Common;
using Documentor.Core.Interfaces;
using Documentor.Core.Models.Department;
using Documentor.Presentation.ViewModels.Base;

namespace Documentor.Presentation.ViewModels.Dialogs.Department;

public class EditDepartmentDialogViewModel : DialogViewModelBase
{
    private readonly IDepartmentManagementService _departmentManagementService;
    private readonly int _departmentId;

    private string _title = string.Empty;
    private string _description = string.Empty;

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

    public ICommand SaveCommand { get; }

    public EditDepartmentDialogViewModel(
        IDepartmentManagementService departmentManagementService,
        int departmentId,
        DepartmentListItemModel department)
    {
        _departmentManagementService = departmentManagementService;
        _departmentId = departmentId;

        Title = department.Title;
        Description = department.Description;

        SaveCommand = new AsyncRelayCommand(SaveAsync);
    }

    private async Task SaveAsync()
    {
        try
        {
            ErrorMessage = string.Empty;
            IsBusy = true;

            await _departmentManagementService.UpdateDepartmentAsync(_departmentId, new EditDepartmentModel
            {
                Title = Title,
                Description = Description
            });

            RequestClose(true);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка обновления отдела: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}