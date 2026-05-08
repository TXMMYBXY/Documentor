namespace Documentor.Core.Models.Department;

public class DepartmentFilterModel
{
    public DepartmentSortField? SortBy { get; set; }
    public bool Descending { get; set; }
    
    public string? Title { get; set; }
    
    public int PageSize { get; set; } = 10;
    public int PageNumber { get; set; } = 1;
}