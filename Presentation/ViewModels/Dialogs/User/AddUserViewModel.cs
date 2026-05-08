using System.Collections.ObjectModel;
using System.Windows.Input;
using Documentor.Common;
using Documentor.Core.Interfaces;
using Documentor.Core.Models;
using Documentor.Core.Models.User;
using Documentor.Presentation.ViewModels.Base;

namespace Documentor.Presentation.ViewModels.Dialogs.User;

public class AddUserViewModel : DialogViewModelBase
{
    private readonly IUserManagementService _userManagementService;

    private string _email = string.Empty;
    private string _fullName = string.Empty;
    private string _password = string.Empty;
    private LookupItemModel? _selectedDepartment;
    private LookupItemModel? _selectedRole;

    public string Email
    {
        get => _email;
        set
        {
            if (SetProperty(ref _email, value))
                (AddCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
        }
    }

    public string FullName
    {
        get => _fullName;
        set
        {
            if (SetProperty(ref _fullName, value))
                (AddCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            if (SetProperty(ref _password, value))
                (AddCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
        }
    }

    public LookupItemModel? SelectedDepartment
    {
        get => _selectedDepartment;
        set
        {
            if (SetProperty(ref _selectedDepartment, value))
                (AddCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
        }
    }

    public LookupItemModel? SelectedRole
    {
        get => _selectedRole;
        set
        {
            if (SetProperty(ref _selectedRole, value))
                (AddCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
        }
    }

    public ObservableCollection<LookupItemModel> Departments { get; } = new();
    public ObservableCollection<LookupItemModel> Roles { get; } = new();

    public ICommand AddCommand { get; }

    public AddUserViewModel(IUserManagementService userManagementService)
    {
        _userManagementService = userManagementService;

        AddCommand = new AsyncRelayCommand(AddAsync, CanAddUser);

        _ = LoadLookupsAsync();
    }

    private async Task LoadLookupsAsync()
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

    private bool CanAddUser()
    {
        return !string.IsNullOrWhiteSpace(Email) &&
               !string.IsNullOrWhiteSpace(FullName) &&
               !string.IsNullOrWhiteSpace(Password) &&
               SelectedRole != null &&
               SelectedDepartment != null;
    }

    private async Task AddAsync()
    {
        try
        {
            ErrorMessage = string.Empty;
            IsBusy = true;

            if (!CanAddUser())
            {
                ErrorMessage = "Заполните все обязательные поля.";
                return;
            }

            var model = new CreateUserModel
            {
                Email = Email,
                FullName = FullName,
                Password = Password,
                Role = SelectedRole!.Id,
                DepartmentId = SelectedDepartment!.Id
            };

            await _userManagementService.CreateUserAsync(model);
            RequestClose(true);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка добавления пользователя: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}