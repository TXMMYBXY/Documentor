using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using DocumentFlowing.Common;
using DocumentFlowing.Presentation.ViewModels.Base;
using Documentor.Core.Interfaces;
using Documentor.Core.Models;
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
        set
        {
            if (SetProperty(ref _selectedUser, value))
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

    public bool IsEmpty => Users.Count == 0;
    public bool CanGoPrevious => CurrentPage > 1;
    public bool CanGoNext => CurrentPage < TotalPages;

    public string ActiveFilterSummary => _BuildFilterSummary();

    public ICommand RefreshCommand { get; }
    public ICommand OpenFilterCommand { get; }
    public ICommand ClearFilterCommand { get; }
    public ICommand AddUserCommand { get; }
    public ICommand EditUserCommand { get; }
    public ICommand ResetPasswordCommand { get; }
    public ICommand ChangeStatusCommand { get; }
    public ICommand DeleteUserCommand { get; }
    public ICommand NextPageCommand { get; }
    public ICommand PreviousPageCommand { get; }

    public UsersPageViewModel(IUserManagementService userManagementService)
    {
        _userManagementService = userManagementService;

        RefreshCommand = new AsyncRelayCommand(_LoadAsync);
        OpenFilterCommand = new RelayCommand(_OpenFilter);
        ClearFilterCommand = new AsyncRelayCommand(_ClearFilterAsync);
        AddUserCommand = new RelayCommand(_AddUser);
        EditUserCommand = new RelayCommand(_EditUser, () => SelectedUser != null);
        ResetPasswordCommand = new RelayCommand(_OpenResetPasswordDialog, () => SelectedUser != null);
        ChangeStatusCommand = new AsyncRelayCommand(_ChangeStatusAsync, () => SelectedUser != null);
        DeleteUserCommand = new AsyncRelayCommand(_DeleteUserAsync, () => SelectedUser != null);
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
            _RaisePagingStateChanged();
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

    private void _OpenFilter()
    {
        var vm = new UserFilterDialogViewModel(_userManagementService, new UserFilterModel
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
            _ = _LoadAsync();
        }
    }

    private async Task _ClearFilterAsync()
    {
        _currentFilter = new UserFilterModel
        {
            PageNumber = 1,
            PageSize = 10
        };

        CurrentPage = 1;
        PageSize = 10;

        await _LoadAsync();
    }

    private void _AddUser()
    {
        var vm = new AddUserViewModel(_userManagementService);

        var dialog = new AddUserDialogWindow
        {
            DataContext = vm,
            Owner = System.Windows.Application.Current.MainWindow
        };

        var result = dialog.ShowDialog();
        if (result == true)
        {
            _ = _LoadAsync();
        }
    }

    private void _EditUser()
    {
        if (SelectedUser == null)
            return;

        var vm = new EditUserDialogViewModel(_userManagementService, SelectedUser.Id, SelectedUser);

        var dialog = new EditUserDialogWindow
        {
            DataContext = vm,
            Owner = System.Windows.Application.Current.MainWindow
        };

        var result = dialog.ShowDialog();
        if (result == true)
        {
            _ = _LoadAsync();
        }
    }

    private void _OpenResetPasswordDialog()
    {
        if (SelectedUser == null)
            return;

        var vm = new ResetPasswordDialogViewModel(_userManagementService, SelectedUser.Id);

        var dialog = new ResetPasswordDialogWindow
        {
            DataContext = vm,
            Owner = System.Windows.Application.Current.MainWindow
        };

        var result = dialog.ShowDialog();
        if (result == true)
        {
            MessageBox.Show("Пароль успешно изменён.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }
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

    private async Task _ChangeStatusAsync()
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

    private async Task _DeleteUserAsync()
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

            await _LoadAsync();
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

    private string _BuildFilterSummary()
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
        if (EditUserCommand is RelayCommand edit)
            edit.RaiseCanExecuteChanged();

        if (ResetPasswordCommand is RelayCommand reset)
            reset.RaiseCanExecuteChanged();

        if (ChangeStatusCommand is AsyncRelayCommand changeStatus)
            changeStatus.RaiseCanExecuteChanged();

        if (DeleteUserCommand is AsyncRelayCommand delete)
            delete.RaiseCanExecuteChanged();
    }
}