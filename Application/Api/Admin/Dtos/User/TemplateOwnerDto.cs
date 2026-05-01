using Documentor.Application.Api.Authorization.Dtos;

namespace Documentor.Application.Api.Admin.Dtos.User;

public class TemplateOwnerDto
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public RoleDto Role { get; set; }
}