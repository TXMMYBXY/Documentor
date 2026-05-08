using System.Collections.ObjectModel;
using System.Windows.Input;
using Documentor.Common;
using Documentor.Core.Interfaces;
using Documentor.Core.Models;
using Documentor.Core.Models.Department;
using Documentor.Presentation.ViewModels.Base;
using Documentor.Presentation.ViewModels.Dialogs.Common;
using Documentor.Presentation.ViewModels.Dialogs.Department;
using Documentor.Presentation.Views.Dialogs.Common;
using Documentor.Presentation.Views.Dialogs.Department;

namespace Documentor.Presentation.ViewModels.Pages;

public class DepartmentsPageViewModel : PagedListPageViewModel<DepartmentListItemModel, DepartmentFilterModel>
{
    private readonly IDepartmentManagementService _departmentManagementService;
    private readonly IAppSettingsService _appSettingsService;

    public string Title => "Управление отделами";

    public ObservableCollection<DepartmentListItemModel> Departments => Items;

    public DepartmentListItemModel? SelectedDepartment
    {
        get => SelectedItem;
        set => SelectedItem = value;
    }

    public override string ActiveFilterSummary => BuildFilterSummary();

    public ICommand OpenFilterCommand { get; }
    public ICommand AddDepartmentCommand { get; }
    public ICommand EditDepartmentCommand { get; }
    public ICommand DeleteDepartmentCommand { get; }

    public DepartmentsPageViewModel(
        IDepartmentManagementService departmentManagementService,
        IAppSettingsService appSettingsService)
    {
        _departmentManagementService = departmentManagementService;
        _appSettingsService = appSettingsService;

        InitializePageSize(_appSettingsService.GetPageSize());

        CurrentFilter = new DepartmentFilterModel
        {
            PageNumber = 1,
            PageSize = PageSize
        };

        OpenFilterCommand = new RelayCommand(OpenFilter);
        AddDepartmentCommand = new RelayCommand(AddDepartment);
        EditDepartmentCommand = new RelayCommand(EditDepartment, () => SelectedDepartment != null);
        DeleteDepartmentCommand = new AsyncRelayCommand(DeleteDepartmentAsync, () => SelectedDepartment != null);

        _ = LoadAsync();
    }
    
    public void ApplySorting(string sortMemberPath, bool descending)
    {
        CurrentFilter.SortBy = sortMemberPath switch
        {
            nameof(DepartmentListItemModel.Title) => DepartmentSortField.Title,
            nameof(DepartmentListItemModel.EmployeesCount) => DepartmentSortField.EmployeesCount,
            _ => DepartmentSortField.Title
        };

        CurrentFilter.Descending = descending;
        CurrentPage = 1;

        _ = LoadAsync();
    }

    protected override void ApplyPagingToFilter()
    {
        CurrentFilter.PageNumber = CurrentPage;
        CurrentFilter.PageSize = PageSize;
    }

    protected override async Task<PagedResult<DepartmentListItemModel>> LoadPageAsync()
    {
        var result = await _departmentManagementService.GetDepartmentAsync(CurrentFilter);

        return new PagedResult<DepartmentListItemModel>
        {
            Items = result.Items.ToList(),
            TotalCount = result.TotalCount,
            TotalPages = result.TotalPages,
            CurrentPage = result.CurrentPage,
            PageSize = result.PageSize
        };
    }

    protected override async Task ClearFilterAsync()
    {
        var pageSize = _appSettingsService.GetPageSize();

        CurrentFilter = new DepartmentFilterModel
        {
            PageNumber = 1,
            PageSize = pageSize
        };

        CurrentPage = 1;
        PageSize = pageSize;

        await LoadAsync();
    }

    protected override void RaiseSelectionCommands()
    {
        if (EditDepartmentCommand is RelayCommand edit)
            edit.RaiseCanExecuteChanged();

        if (DeleteDepartmentCommand is AsyncRelayCommand delete)
            delete.RaiseCanExecuteChanged();
    }

    protected override string BuildLoadErrorMessage(Exception ex)
    {
        return $"Ошибка загрузки отделов: {ex.Message}";
    }

    private void OpenFilter()
    {
        DepartmentFilterDialogWindow? dialog = null;

        var vm = new DepartmentFilterDialogViewModel(new DepartmentFilterModel
        {
            Title = CurrentFilter.Title,
            PageSize = CurrentFilter.PageSize
        });

        vm.CloseRequested = result => dialog!.DialogResult = result;

        dialog = new DepartmentFilterDialogWindow
        {
            DataContext = vm,
            Owner = System.Windows.Application.Current.MainWindow
        };

        var result = dialog.ShowDialog();
        if (result == true)
        {
            CurrentFilter = vm.ResultFilter;
            CurrentPage = 1;
            PageSize = CurrentFilter.PageSize;
            _ = LoadAsync();
        }
    }

    private void AddDepartment()
    {
        AddDepartmentDialogWindow? dialog = null;

        var vm = new AddDepartmentViewModel(_departmentManagementService);
        vm.CloseRequested = result => dialog!.DialogResult = result;

        dialog = new AddDepartmentDialogWindow
        {
            DataContext = vm,
            Owner = System.Windows.Application.Current.MainWindow
        };

        var result = dialog.ShowDialog();
        if (result == true)
        {
            _ = LoadAsync();
        }
    }

    private void EditDepartment()
    {
        if (SelectedDepartment == null)
            return;

        EditDepartmentDialogWindow? dialog = null;

        var vm = new EditDepartmentDialogViewModel(
            _departmentManagementService,
            SelectedDepartment.Id,
            SelectedDepartment);

        vm.CloseRequested = result => dialog!.DialogResult = result;

        dialog = new EditDepartmentDialogWindow
        {
            DataContext = vm,
            Owner = System.Windows.Application.Current.MainWindow
        };

        var result = dialog.ShowDialog();
        if (result == true)
        {
            _ = LoadAsync();
        }
    }

    public void OpenEmployeesDialogFor(DepartmentListItemModel? department)
    {
        if (department == null)
            return;

        DepartmentEmployeesDialogWindow? dialog = null;

        var vm = new DepartmentEmployeesDialogViewModel(department, () => dialog?.Close());

        dialog = new DepartmentEmployeesDialogWindow
        {
            DataContext = vm,
            Owner = System.Windows.Application.Current.MainWindow
        };

        dialog.ShowDialog();
    }

    private async Task DeleteDepartmentAsync()
    {
        if (SelectedDepartment == null)
            return;

        ConfirmationDialogWindow? dialog = null;

        var vm = new ConfirmationDialogViewModel(
            "Удаление отдела",
            $"Удалить отдел \"{SelectedDepartment.Title}\"?",
            result => dialog!.DialogResult = result,
            "Удалить",
            "Отмена");

        dialog = new ConfirmationDialogWindow
        {
            DataContext = vm,
            Owner = System.Windows.Application.Current.MainWindow
        };

        var confirm = dialog.ShowDialog();
        if (confirm != true)
            return;

        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            await _departmentManagementService.DeleteDepartmentAsync(SelectedDepartment.Id);

            if (Departments.Count == 1 && CurrentPage > 1)
            {
                CurrentPage--;
            }

            await LoadAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка удаления отдела: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private string BuildFilterSummary()
    {
        var parts = new List<string>();

        if (!string.IsNullOrWhiteSpace(CurrentFilter.Title))
            parts.Add($"Название: {CurrentFilter.Title}");

        return parts.Count == 0
            ? "Фильтр не применён"
            : string.Join(" | ", parts);
    }
}