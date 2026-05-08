using Documentor.Core.Enums;

namespace Documentor.Core.Models.User;

public class UserInfoModel
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public Role Role { get; set; }
    public string RoleTitle { get; set; } = string.Empty;
}