namespace Documentor.Core.Models;

public class EditUserModel
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public int RoleId { get; set; }
}