using System.Collections.ObjectModel;
using System.Windows.Input;
using Documentor.Common;
using Documentor.Core.Interfaces;
using Documentor.Core.Models;
using Documentor.Core.Models.User;
using Documentor.Presentation.ViewModels.Base;

namespace Documentor.Presentation.ViewModels.Dialogs.User;

public class EditUserDialogViewModel : DialogViewModelBase
{
    private readonly IUserManagementService _userManagementService;
    private readonly int _userId;

    private string _fullName = string.Empty;
    private string _email = string.Empty;
    private LookupItemModel? _selectedDepartment;
    private LookupItemModel? _selectedRole;

    public string FullName
    {
        get => _fullName;
        set => SetProperty(ref _fullName, value);
    }

    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    public LookupItemModel? SelectedDepartment
    {
        get => _selectedDepartment;
        set => SetProperty(ref _selectedDepartment, value);
    }

    public LookupItemModel? SelectedRole
    {
        get => _selectedRole;
        set => SetProperty(ref _selectedRole, value);
    }

    public ObservableCollection<LookupItemModel> Departments { get; } = new();
    public ObservableCollection<LookupItemModel> Roles { get; } = new();

    public ICommand SaveCommand { get; }

    public EditUserDialogViewModel(
        IUserManagementService userManagementService,
        int userId,
        UserListItemModel user)
    {
        _userManagementService = userManagementService;
        _userId = userId;

        FullName = user.FullName;
        Email = user.Email;

        SaveCommand = new AsyncRelayCommand(SaveAsync);

        _ = LoadLookupsAsync(user);
    }

    private async Task LoadLookupsAsync(UserListItemModel user)
    {
        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            var departments = await _userManagementService.GetDepartmentsAsync();
            var roles = await _userManagementService.GetRolesAsync();

            Departments.Clear();
            foreach (var item in departments)
                Departments.Add(item);

            Roles.Clear();
            foreach (var item in roles)
                Roles.Add(item);

            SelectedDepartment = Departments.FirstOrDefault(x => x.Title == user.Department);
            SelectedRole = Roles.FirstOrDefault(x => x.Title == user.Role);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка загрузки справочников: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task SaveAsync()
    {
        try
        {
            ErrorMessage = string.Empty;
            IsBusy = true;

            if (string.IsNullOrWhiteSpace(FullName) ||
                string.IsNullOrWhiteSpace(Email) ||
                SelectedDepartment == null ||
                SelectedRole == null)
            {
                ErrorMessage = "Заполните все поля";
                return;
            }

            await _userManagementService.UpdateUserAsync(_userId, new EditUserModel
            {
                FullName = FullName,
                Email = Email,
                DepartmentId = SelectedDepartment.Id,
                RoleId = SelectedRole.Id
            });

            RequestClose(true);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка сохранения: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}