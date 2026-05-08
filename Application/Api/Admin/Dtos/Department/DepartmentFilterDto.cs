using Documentor.Core.Models.Department;

namespace Documentor.Application.Api.Admin.Dtos.Department;

public class DepartmentFilterDto
{
    public DepartmentSortField? SortBy { get; set; }
    public bool Descending { get; set; }
    
    public string Title { get; set; } = string.Empty;
    
    public int? PageSize { get; set; } = 10;
    public int? PageNumber { get; set; } = 1;
}