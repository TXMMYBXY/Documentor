using Documentor.Core.Enums;

namespace Documentor.Application.Api.Admin.Dtos.User;

public class UpdateUserDto
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public string DepartmentId { get; set; }
    public Role Role { get; set; }
}