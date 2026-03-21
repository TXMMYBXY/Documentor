namespace Documentor.Application.Api.Admin.Dtos;

public class UserFilterDto
{
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public int? DepartmentId { get; set; }
    public int? RoleId { get; set; }

    public int? PageSize { get; set; }
    public int? PageNumber { get; set; }
}