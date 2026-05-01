using System.Collections.ObjectModel;
using System.Windows.Input;
using Documentor.Common;
using Documentor.Core.Interfaces;
using Documentor.Core.Models;
using Documentor.Core.Models.User;
using Documentor.Presentation.ViewModels.Base;

namespace Documentor.Presentation.ViewModels.Dialogs.User;

public class UserFilterDialogViewModel : DialogViewModelBase
{
    private readonly IUserManagementService _userManagementService;

    private string? _fullName;
    private string? _email;
    private LookupItemModel? _selectedDepartment;
    private LookupItemModel? _selectedRole;
    private int _pageSize = 10;

    public string? FullName
    {
        get => _fullName;
        set => SetProperty(ref _fullName, value);
    }

    public string? Email
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

    public int PageSize
    {
        get => _pageSize;
        set => SetProperty(ref _pageSize, value);
    }

    public ObservableCollection<LookupItemModel> Departments { get; } = new();
    public ObservableCollection<LookupItemModel> Roles { get; } = new();

    public ICommand ApplyCommand { get; }
    public ICommand ResetCommand { get; }

    public UserFilterModel ResultFilter => new()
    {
        FullName = FullName,
        Email = Email,
        DepartmentId = SelectedDepartment?.Id == 0 ? null : SelectedDepartment?.Id,
        RoleId = SelectedRole?.Id == 0 ? null : SelectedRole?.Id,
        PageSize = PageSize,
        PageNumber = 1
    };

    public UserFilterDialogViewModel(
        IUserManagementService userManagementService,
        UserFilterModel? currentFilter = null)
    {
        _userManagementService = userManagementService;

        if (currentFilter != null)
        {
            FullName = currentFilter.FullName;
            Email = currentFilter.Email;
            PageSize = currentFilter.PageSize == 0 ? 10 : currentFilter.PageSize;
        }

        ApplyCommand = new RelayCommand(() => RequestClose(true));
        ResetCommand = new RelayCommand(Reset);

        _ = LoadLookupsAsync(currentFilter);
    }

    private async Task LoadLookupsAsync(UserFilterModel? currentFilter)
    {
        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            var departments = await _userManagementService.GetDepartmentsAsync();
            var roles = await _userManagementService.GetRolesAsync();

            Departments.Clear();
            Departments.Add(new LookupItemModel { Id = 0, Title = "Все" });
            foreach (var item in departments)
                Departments.Add(item);

            Roles.Clear();
            Roles.Add(new LookupItemModel { Id = 0, Title = "Все" });
            foreach (var item in roles)
                Roles.Add(item);

            if (currentFilter?.DepartmentId != null)
                SelectedDepartment = Departments.FirstOrDefault(x => x.Id == currentFilter.DepartmentId);
            else
                SelectedDepartment = Departments.FirstOrDefault();

            if (currentFilter?.RoleId != null)
                SelectedRole = Roles.FirstOrDefault(x => x.Id == currentFilter.RoleId);
            else
                SelectedRole = Roles.FirstOrDefault();
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

    private void Reset()
    {
        FullName = null;
        Email = null;
        SelectedDepartment = Departments.FirstOrDefault();
        SelectedRole = Roles.FirstOrDefault();
        PageSize = 10;
    }
}