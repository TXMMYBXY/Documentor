using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using DocumentFlowing.Common;
using DocumentFlowing.Presentation.ViewModels.Base;
using Documentor.Core.Interfaces;
using Documentor.Core.Models;
using Documentor.Core.Services;
using Documentor.Presentation.ViewModels.Dialogs;
using Documentor.Presentation.Views.Dialogs;

namespace Documentor.Presentation.ViewModels.Pages;

public class UsersPageViewModel : ViewModelBase
{
    private readonly IUserManagementService _userManagementService;

    private UserListItemModel? _selectedUser;
    private string _errorMessage = string.Empty;
    private bool _isLoading;

    private int _currentPage = 1;
    private int _pageSize = 10;
    private int _totalPages;
    private int _totalCount;

    private UserFilterModel _currentFilter = new();

    public string Title => "Управление пользователями";

    public ObservableCollection<UserListItemModel> Users { get; } = new();

    public UserListItemModel? SelectedUser
    {
        get => _selectedUser;
        set => SetProperty(ref _selectedUser, value);
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

    public bool IsEmpty => Users.Count == 0;
    public bool CanGoPrevious => CurrentPage > 1;
    public bool CanGoNext => CurrentPage < TotalPages;

    public string ActiveFilterSummary =>
        BuildFilterSummary();

    public ICommand RefreshCommand { get; }
    public ICommand OpenFilterCommand { get; }
    public ICommand ClearFilterCommand { get; }
    public ICommand ChangeStatusCommand { get; }
    public ICommand DeleteUserCommand { get; }
    public ICommand NextPageCommand { get; }
    public ICommand PreviousPageCommand { get; }

    public UsersPageViewModel(IUserManagementService userManagementService)
    {
        _userManagementService = userManagementService;

        RefreshCommand = new AsyncRelayCommand(LoadAsync);
        OpenFilterCommand = new RelayCommand(OpenFilter);
        ClearFilterCommand = new AsyncRelayCommand(ClearFilterAsync);
        ChangeStatusCommand = new AsyncRelayCommand(ChangeStatusAsync, () => SelectedUser != null);
        DeleteUserCommand = new AsyncRelayCommand(DeleteUserAsync, () => SelectedUser != null);
        NextPageCommand = new AsyncRelayCommand(NextPageAsync, () => CanGoNext);
        PreviousPageCommand = new AsyncRelayCommand(PreviousPageAsync, () => CanGoPrevious);

        _ = LoadAsync();
    }

    private async Task LoadAsync()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            _currentFilter.PageNumber = CurrentPage;
            _currentFilter.PageSize = PageSize;

            var result = await _userManagementService.GetUsersAsync(_currentFilter);

            Users.Clear();
            foreach (var user in result.Items)
            {
                Users.Add(user);
            }

            TotalCount = result.TotalCount;
            TotalPages = result.TotalPages;
            CurrentPage = result.CurrentPage == 0 ? 1 : result.CurrentPage;
            PageSize = result.PageSize == 0 ? PageSize : result.PageSize;

            OnPropertyChanged(nameof(IsEmpty));
            OnPropertyChanged(nameof(ActiveFilterSummary));
            RaisePagingStateChanged();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка загрузки пользователей: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void OpenFilter()
    {
        var vm = new UserFilterDialogViewModel(new UserFilterModel
        {
            FullName = _currentFilter.FullName,
            Email = _currentFilter.Email,
            DepartmentId = _currentFilter.DepartmentId,
            RoleId = _currentFilter.RoleId,
            PageSize = _currentFilter.PageSize
        });

        var dialog = new UserFilterDialogWindow
        {
            DataContext = vm,
            Owner = System.Windows.Application.Current.MainWindow
        };

        var result = dialog.ShowDialog();
        if (result == true)
        {
            _currentFilter = vm.ResultFilter;
            CurrentPage = 1;
            PageSize = _currentFilter.PageSize;
            _ = LoadAsync();
        }
    }

    private async Task ClearFilterAsync()
    {
        _currentFilter = new UserFilterModel
        {
            PageNumber = 1,
            PageSize = 10
        };

        CurrentPage = 1;
        PageSize = 10;

        await LoadAsync();
    }

    private async Task NextPageAsync()
    {
        if (!CanGoNext)
            return;

        CurrentPage++;
        await LoadAsync();
    }

    private async Task PreviousPageAsync()
    {
        if (!CanGoPrevious)
            return;

        CurrentPage--;
        await LoadAsync();
    }

    private async Task ChangeStatusAsync()
    {
        if (SelectedUser == null)
            return;

        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            var newStatus = await _userManagementService.ChangeStatusAsync(SelectedUser.Id);
            SelectedUser.IsActive = newStatus;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка изменения статуса: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task DeleteUserAsync()
    {
        if (SelectedUser == null)
            return;

        var result = MessageBox.Show(
            $"Удалить пользователя \"{SelectedUser.FullName}\"?",
            "Подтверждение удаления",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (result != MessageBoxResult.Yes)
            return;

        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            await _userManagementService.DeleteUserAsync(SelectedUser.Id);

            if (Users.Count == 1 && CurrentPage > 1)
            {
                CurrentPage--;
            }

            await LoadAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка удаления пользователя: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private string BuildFilterSummary()
    {
        var parts = new List<string>();

        if (!string.IsNullOrWhiteSpace(_currentFilter.FullName))
            parts.Add($"ФИО: {_currentFilter.FullName}");

        if (!string.IsNullOrWhiteSpace(_currentFilter.Email))
            parts.Add($"Email: {_currentFilter.Email}");

        if (_currentFilter.DepartmentId.HasValue)
            parts.Add($"Отдел ID: {_currentFilter.DepartmentId}");

        if (_currentFilter.RoleId.HasValue)
            parts.Add($"Роль ID: {_currentFilter.RoleId}");

        return parts.Count == 0
            ? "Фильтр не применён"
            : string.Join(" | ", parts);
    }

    private void RaisePagingStateChanged()
    {
        OnPropertyChanged(nameof(CanGoPrevious));
        OnPropertyChanged(nameof(CanGoNext));

        if (NextPageCommand is AsyncRelayCommand next)
            next.RaiseCanExecuteChanged();

        if (PreviousPageCommand is AsyncRelayCommand prev)
            prev.RaiseCanExecuteChanged();
    }
}