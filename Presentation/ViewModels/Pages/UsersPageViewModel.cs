using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using DocumentFlowing.Common;
using Documentor.Core.Interfaces;
using Documentor.Core.Models;
using Documentor.Core.Models.User;
using Documentor.Presentation.ViewModels.Base;
using Documentor.Presentation.ViewModels.Dialogs.Common;
using Documentor.Presentation.ViewModels.Dialogs.User;
using Documentor.Presentation.Views.Dialogs;
using Documentor.Presentation.Views.Dialogs.Common;

namespace Documentor.Presentation.ViewModels.Pages;

public class UsersPageViewModel : PagedListPageViewModel<UserListItemModel, UserFilterModel>
{
    private readonly IUserManagementService _userManagementService;
    private readonly IAppSettingsService _appSettingsService;

    public static string Title => "Управление пользователями";

    public ObservableCollection<UserListItemModel> Users => Items;

    public UserListItemModel? SelectedUser
    {
        get => SelectedItem;
        set => SelectedItem = value;
    }

    public override string ActiveFilterSummary => BuildFilterSummary();

    public ICommand OpenFilterCommand { get; }
    public ICommand AddUserCommand { get; }
    public ICommand EditUserCommand { get; }
    public ICommand ResetPasswordCommand { get; }
    public ICommand ChangeStatusCommand { get; }
    public ICommand DeleteUserCommand { get; }

    public UsersPageViewModel(
        IUserManagementService userManagementService,
        IAppSettingsService appSettingsService)
    {
        _userManagementService = userManagementService;
        _appSettingsService = appSettingsService;

        InitializePageSize(_appSettingsService.GetPageSize());

        CurrentFilter = new UserFilterModel
        {
            PageNumber = 1,
            PageSize = PageSize
        };

        OpenFilterCommand = new RelayCommand(OpenFilter);
        AddUserCommand = new RelayCommand(AddUser);
        EditUserCommand = new RelayCommand(EditUser, () => SelectedUser != null);
        ResetPasswordCommand = new RelayCommand(OpenResetPasswordDialog, () => SelectedUser != null);
        ChangeStatusCommand = new AsyncRelayCommand(ChangeStatusAsync, () => SelectedUser != null);
        DeleteUserCommand = new AsyncRelayCommand(DeleteUserAsync, () => SelectedUser != null);

        _ = LoadAsync();
    }

    protected override void ApplyPagingToFilter()
    {
        CurrentFilter.PageNumber = CurrentPage;
        CurrentFilter.PageSize = PageSize;
    }

    protected override async Task<PagedResult<UserListItemModel>> LoadPageAsync()
    {
        var result = await _userManagementService.GetUsersAsync(CurrentFilter);

        return new PagedResult<UserListItemModel>
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

        CurrentFilter = new UserFilterModel
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
        if (EditUserCommand is RelayCommand edit)
            edit.RaiseCanExecuteChanged();

        if (ResetPasswordCommand is RelayCommand reset)
            reset.RaiseCanExecuteChanged();

        if (ChangeStatusCommand is AsyncRelayCommand changeStatus)
            changeStatus.RaiseCanExecuteChanged();

        if (DeleteUserCommand is AsyncRelayCommand delete)
            delete.RaiseCanExecuteChanged();
    }

    protected override string BuildLoadErrorMessage(Exception ex)
    {
        return $"Ошибка загрузки пользователей: {ex.Message}";
    }

    private void OpenFilter()
    {
        UserFilterDialogWindow? dialog = null;

        var vm = new UserFilterDialogViewModel(_userManagementService, new UserFilterModel
        {
            FullName = CurrentFilter.FullName,
            Email = CurrentFilter.Email,
            DepartmentId = CurrentFilter.DepartmentId,
            RoleId = CurrentFilter.RoleId,
            PageSize = CurrentFilter.PageSize
        });

        vm.CloseRequested = result => dialog!.DialogResult = result;

        dialog = new UserFilterDialogWindow
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

    private void AddUser()
    {
        AddUserDialogWindow? dialog = null;

        var vm = new AddUserViewModel(_userManagementService);
        vm.CloseRequested = result => dialog!.DialogResult = result;

        dialog = new AddUserDialogWindow
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

    private void EditUser()
    {
        if (SelectedUser == null)
            return;

        EditUserDialogWindow? dialog = null;

        var vm = new EditUserDialogViewModel(_userManagementService, SelectedUser.Id, SelectedUser);
        vm.CloseRequested = result => dialog!.DialogResult = result;

        dialog = new EditUserDialogWindow
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

    private void OpenResetPasswordDialog()
    {
        if (SelectedUser == null)
            return;

        ResetPasswordDialogWindow? dialog = null;

        var vm = new ResetPasswordDialogViewModel(_userManagementService, SelectedUser.Id);
        vm.CloseRequested = result => dialog!.DialogResult = result;

        dialog = new ResetPasswordDialogWindow
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

        ConfirmationDialogWindow? dialog = null;

        var vm = new ConfirmationDialogViewModel(
            "Удаление пользователя",
            $"Удалить пользователя \"{SelectedUser.FullName}\"?",
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

        if (!string.IsNullOrWhiteSpace(CurrentFilter.FullName))
            parts.Add($"ФИО: {CurrentFilter.FullName}");

        if (!string.IsNullOrWhiteSpace(CurrentFilter.Email))
            parts.Add($"Email: {CurrentFilter.Email}");

        if (CurrentFilter.DepartmentId.HasValue)
            parts.Add($"Отдел ID: {CurrentFilter.DepartmentId}");

        if (CurrentFilter.RoleId.HasValue)
            parts.Add($"Роль ID: {CurrentFilter.RoleId}");

        return parts.Count == 0
            ? "Фильтр не применён"
            : string.Join(" | ", parts);
    }
}