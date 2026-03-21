using System.Windows.Input;
using DocumentFlowing.Common;
using DocumentFlowing.Presentation.ViewModels.Base;
using Documentor.Core.Models;

namespace Documentor.Presentation.ViewModels.Dialogs;

public class UserFilterDialogViewModel : ViewModelBase
{
    private string? _fullName;
    private string? _email;
    private int? _departmentId;
    private int? _roleId;
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

    public int? DepartmentId
    {
        get => _departmentId;
        set => SetProperty(ref _departmentId, value);
    }

    public int? RoleId
    {
        get => _roleId;
        set => SetProperty(ref _roleId, value);
    }

    public int PageSize
    {
        get => _pageSize;
        set => SetProperty(ref _pageSize, value);
    }

    public ICommand ApplyCommand { get; }
    public ICommand ResetCommand { get; }
    public ICommand CancelCommand { get; }

    public Action<bool?>? CloseRequested { get; set; }

    public UserFilterModel ResultFilter => new()
    {
        FullName = FullName,
        Email = Email,
        DepartmentId = DepartmentId,
        RoleId = RoleId,
        PageSize = PageSize,
        PageNumber = 1
    };

    public UserFilterDialogViewModel(UserFilterModel? currentFilter = null)
    {
        if (currentFilter != null)
        {
            FullName = currentFilter.FullName;
            Email = currentFilter.Email;
            DepartmentId = currentFilter.DepartmentId;
            RoleId = currentFilter.RoleId;
            PageSize = currentFilter.PageSize;
        }

        ApplyCommand = new RelayCommand(() => CloseRequested?.Invoke(true));
        ResetCommand = new RelayCommand(Reset);
        CancelCommand = new RelayCommand(() => CloseRequested?.Invoke(false));
    }

    private void Reset()
    {
        FullName = null;
        Email = null;
        DepartmentId = null;
        RoleId = null;
        PageSize = 10;
    }
}