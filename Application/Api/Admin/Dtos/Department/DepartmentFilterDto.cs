namespace Documentor.Application.Api.Admin.Dtos.Department;

public class DepartmentFilterDto
{
    public string Title { get; set; } = string.Empty;
    
    public int? PageSize { get; set; } = 10;
    public int? PageNumber { get; set; } = 1;
}