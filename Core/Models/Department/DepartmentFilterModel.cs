namespace Documentor.Core.Models.Department;

public class DepartmentFilterModel
{
    public string? Title { get; set; }
    
    public int PageSize { get; set; } = 10;
    public int PageNumber { get; set; } = 1;
}