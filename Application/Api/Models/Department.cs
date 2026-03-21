using Documentor.Application.Api.Admin.Dtos;

namespace DocumentFlowing.Client.Models;

public class Department
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public virtual ICollection<EmployeeDto>? Employees { get; set; }
}