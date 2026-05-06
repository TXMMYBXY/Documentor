using Documentor.Core.Enums;
using Documentor.Core.Extensions;
using Documentor.Presentation.ViewModels.Base;

namespace Documentor.Core.Models.User;

public class UserListItemModel : ViewModelBase
{
    private int _id;
    private string _email = string.Empty;
    private string _fullName = string.Empty;
    private bool _isActive;
    private string _department = string.Empty;
    private Role _role;

    public int Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    public string FullName
    {
        get => _fullName;
        set => SetProperty(ref _fullName, value);
    }

    public bool IsActive
    {
        get => _isActive;
        set => SetProperty(ref _isActive, value);
    }

    public string Department
    {
        get => _department;
        set => SetProperty(ref _department, value);
    }

    public Role Role
    {
        get => _role;
        set => SetProperty(ref _role, value);
    }

    public string RoleDisplay => Role.GetDisplayName();
}