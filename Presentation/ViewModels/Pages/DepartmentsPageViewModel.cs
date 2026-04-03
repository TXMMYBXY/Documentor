using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using DocumentFlowing.Common;
using DocumentFlowing.Presentation.ViewModels.Base;
using Documentor.Core.Interfaces;
using Documentor.Core.Models.Department;
using Documentor.Presentation.ViewModels.Dialogs.Department;
using Documentor.Presentation.Views.Dialogs.Department;

namespace Documentor.Presentation.ViewModels.Pages;

public class DepartmentsPageViewModel : ViewModelBase
{
    private readonly IDepartmentManagementService _departmentManagementService;

    private DepartmentListItemModel? _selectedDepartment;
    private string _errorMessage = string.Empty;
    private bool _isLoading;

    private int _currentPage = 1;
    private int _pageSize = 10;
    private int _totalPages;
    private int _totalCount;

    private DepartmentFilterModel _currentFilter = new();

    public string Title => "Управление отделами";

    public ObservableCollection<DepartmentListItemModel> Departments { get; } = new();

    public DepartmentListItemModel? SelectedDepartment
    {
        get => _selectedDepartment;
        set
        {
            if (SetProperty(ref _selectedDepartment, value))
            {
                _RaiseSelectionCommands();
            }
        }
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public int CurrentPage
    {
        get => _currentPage;
        set => SetProperty(ref _currentPage, value);
    }

    public int PageSize
    {
        get => _pageSize;
        set => SetProperty(ref _pageSize, value);
    }

    public int TotalPages
    {
        get => _totalPages;
        set => SetProperty(ref _totalPages, value);
    }

    public int TotalCount
    {
        get => _totalCount;
        set => SetProperty(ref _totalCount, value);
    }

    public bool IsEmpty => Departments.Count == 0;
    public bool CanGoPrevious => CurrentPage > 1;
    public bool CanGoNext => CurrentPage < TotalPages;

    public ICommand RefreshCommand { get; }
    public ICommand OpenFilterCommand { get; }
    public ICommand ClearFilterCommand { get; }
    public ICommand AddDepartmentCommand { get; }
    public ICommand EditDepartmentCommand { get; }
    public ICommand DeleteDepartmentCommand { get; }
    public ICommand NextPageCommand { get; }
    public ICommand PreviousPageCommand { get; }

    public string ActiveFilterSummary => _BuildFilterSummary();

    public DepartmentsPageViewModel(IDepartmentManagementService departmentManagementService)
    {
        _departmentManagementService = departmentManagementService;

        RefreshCommand = new AsyncRelayCommand(_LoadAsync);
        OpenFilterCommand = new RelayCommand(_OpenFilterStub);
        ClearFilterCommand = new AsyncRelayCommand(_ClearFilterAsync);
        AddDepartmentCommand = new RelayCommand(_AddDepartmentStub);
        EditDepartmentCommand = new RelayCommand(_EditDepartmentStub, () => SelectedDepartment != null);
        DeleteDepartmentCommand = new AsyncRelayCommand(_DeleteDepartmentAsync, () => SelectedDepartment != null);
        NextPageCommand = new AsyncRelayCommand(_NextPageAsync, () => CanGoNext);
        PreviousPageCommand = new AsyncRelayCommand(_PreviousPageAsync, () => CanGoPrevious);

        _ = _LoadAsync();
    }

    private async Task _LoadAsync()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            _currentFilter.PageNumber = CurrentPage;
            _currentFilter.PageSize = PageSize;

            var result = await _departmentManagementService.GetDepartmentAsync(_currentFilter);

            Departments.Clear();
            foreach (var department in result.Items)
            {
                Departments.Add(department);
            }

            TotalCount = result.TotalCount;
            TotalPages = result.TotalPages;
            CurrentPage = result.CurrentPage == 0 ? 1 : result.CurrentPage;
            PageSize = result.PageSize == 0 ? PageSize : result.PageSize;

            OnPropertyChanged(nameof(IsEmpty));
            OnPropertyChanged(nameof(ActiveFilterSummary));
            _RaisePagingStateChanged();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка загрузки отделов: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void _OpenFilterStub()
    {
        MessageBox.Show("Окно фильтра отделов будет добавлено позже.", "Информация",
            MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private async Task _ClearFilterAsync()
    {
        _currentFilter = new DepartmentFilterModel
        {
            PageNumber = 1,
            PageSize = 10
        };

        CurrentPage = 1;
        PageSize = 10;

        await _LoadAsync();
    }

    private void _AddDepartmentStub()
    {
        MessageBox.Show("Окно добавления отдела будет добавлено позже.", "Информация",
            MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void _EditDepartmentStub()
    {
        if (SelectedDepartment == null)
            return;

        MessageBox.Show($"Окно редактирования отдела «{SelectedDepartment.Title}» будет добавлено позже.",
            "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
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

    private async Task _DeleteDepartmentAsync()
    {
        if (SelectedDepartment == null)
            return;

        var result = MessageBox.Show(
            $"Удалить отдел \"{SelectedDepartment.Title}\"?",
            "Подтверждение удаления",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (result != MessageBoxResult.Yes)
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

            await _LoadAsync();
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

    private string _BuildFilterSummary()
    {
        var parts = new List<string>();

        if (!string.IsNullOrWhiteSpace(_currentFilter.Title))
            parts.Add($"Название: {_currentFilter.Title}");

        return parts.Count == 0
            ? "Фильтр не применён"
            : string.Join(" | ", parts);
    }

    private void _RaisePagingStateChanged()
    {
        OnPropertyChanged(nameof(CanGoPrevious));
        OnPropertyChanged(nameof(CanGoNext));

        if (NextPageCommand is AsyncRelayCommand next)
            next.RaiseCanExecuteChanged();

        if (PreviousPageCommand is AsyncRelayCommand prev)
            prev.RaiseCanExecuteChanged();
    }

    private void _RaiseSelectionCommands()
    {
        if (EditDepartmentCommand is RelayCommand edit)
            edit.RaiseCanExecuteChanged();

        if (DeleteDepartmentCommand is AsyncRelayCommand delete)
            delete.RaiseCanExecuteChanged();
    }

    private async Task _NextPageAsync()
    {
        if (!CanGoNext)
            return;

        CurrentPage++;
        await _LoadAsync();
    }

    private async Task _PreviousPageAsync()
    {
        if (!CanGoPrevious)
            return;

        CurrentPage--;
        await _LoadAsync();
    }
}