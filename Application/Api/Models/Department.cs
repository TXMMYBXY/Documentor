using Documentor.Application.Api.Admin.Dtos.User;

namespace Documentor.Application.Api.Models;

public class Department
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public virtual ICollection<EmployeeDto>? Employees { get; set; }
}