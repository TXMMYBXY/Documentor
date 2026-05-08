namespace Documentor.Core.Models.User;

public class UserFilterModel
{
    public UserSortField? SortBy { get; set; }
    public bool Descending { get; set; }
    
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public int? DepartmentId { get; set; }
    public int? RoleId { get; set; }

    public int PageSize { get; set; } = 10;
    public int PageNumber { get; set; } = 1;
}