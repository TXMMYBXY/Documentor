using Documentor.Core.Enums;
using Documentor.Core.Models;
using Documentor.Core.Models.User;

namespace Documentor.Core.Interfaces;

public interface IUserSession
{
    bool IsAuthenticated { get; }
    int UserId { get; }
    string FullName { get; }
    string Email { get; }
    string Department { get; }
    UserRole Role { get; }
    string RoleTitle { get; }

    void Set(UserInfoModel userInfo);
    void Clear();
}