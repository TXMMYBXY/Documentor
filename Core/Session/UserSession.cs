using Documentor.Core.Enums;
using Documentor.Core.Interfaces;
using Documentor.Core.Models;
using Documentor.Core.Models.User;

namespace Documentor.Core.Session;

public class UserSession : IUserSession
{
    public bool IsAuthenticated { get; private set; }

    public int UserId { get; private set; }
    public string FullName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Department { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }
    public string RoleTitle { get; private set; } = string.Empty;

    public void Set(UserInfoModel userInfo)
    {
        UserId = userInfo.UserId;
        FullName = userInfo.FullName;
        Email = userInfo.Email;
        Department = userInfo.Department;
        Role = userInfo.Role;
        RoleTitle = userInfo.RoleTitle;
        IsAuthenticated = true;
    }

    public void Clear()
    {
        UserId = 0;
        FullName = string.Empty;
        Email = string.Empty;
        Department = string.Empty;
        Role = default;
        RoleTitle = string.Empty;
        IsAuthenticated = false;
    }
}